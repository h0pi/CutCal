# Sistem preporuke — CutCal

Preporuke saloni prikazuju se korisniku (ulozi `Customer`) na početnom ekranu mobilne aplikacije u sekciji **Recommended for you**. Uz svaki salon prikazuje se i **objašnjenje** zašto je preporučen.

- Endpoint: `GET /Recommendations?lat=&lng=` (samo `Customer`, korisnik se uzima iz JWT-a)
- Implementacija: `backend/CutCal.Services/Services/RecommendationService.cs`
- Vrsta: hibridni, objašnjivi recommender — **content-based** (sličnost salona s onim što je korisnik već koristio) uz **popularity-based** i **quality-based** komponentu, te **udaljenost** kada je lokacija poznata.

## 1. Signali koji ulaze u preporuku

Svi signali se stvarno upisuju u bazu tokom korištenja aplikacije i svi se koriste u bodovanju.

| Signal | Gdje se upisuje | Tabela |
|---|---|---|
| Rezervacije korisnika (nisu otkazane) | kreiranje rezervacije | `Appointments` |
| Omiljeni saloni | dodavanje/uklanjanje favorita | `Favorites` |
| Pregled salona (otvaranje detalja) | `POST /Salons/{id}/View`, poziva se pri otvaranju ekrana salona; ponovni pregled istog salona unutar 10 minuta se ne broji | `SalonViews` |
| Pretraga po kategoriji | `GET /Salons?categoryId=` (klik na kategoriju na početnom ekranu) | `UserSearchHistories` |
| Recenzije korisnika | ostavljanje recenzije | `Reviews` |
| Ocjena salona (`AvgRating`) | preračunava se pri svakoj recenziji | `Salons` |
| Popularnost (broj rezervacija svih korisnika) | agregat nad rezervacijama | `Appointments` |
| Lokacija korisnika | šalje je mobilna aplikacija (`lat`, `lng`), ne čuva se | — |

## 2. Snaga interakcije korisnika sa salonom

Za svaki salon računa se koliko je korisniku "drag":

| Interakcija | Doprinos |
|---|---|
| Rezervacija (po komadu, ukupno najviše 6) | +3 |
| Favorit | +2 |
| Pregled (po komadu, ukupno najviše 2) | +0.5 |
| Prosječna ocjena koju je korisnik dao ≥ 4 | +1 |
| Prosječna ocjena koju je korisnik dao ≤ 2 | −2 |

Saloni s ukupnom snagom > 0 su **"sjemena"** (seeds) korisnikovog profila. Saloni s ukupnom snagom ≤ 0 se ne koriste kao sjemena.

## 3. Sličnost dva salona

`sim(A, B) = 0.6 · ista_kategorija + 0.2 · isti_grad + 0.2 · bliskost_cijena`

- `bliskost_cijena = 1 − min(|cA − cB| / max(cA, cB), 1)`, gdje je `c` prosječna cijena aktivnih usluga salona; ako cijena nije poznata, koristi se neutralnih 0.5.
- `sim(A, A) = 1`, pa salon koji je korisnik već koristio, pregledao ili spremio i sam dobija visok rezultat.

## 4. Komponente rezultata (sve u rasponu 0–1)

1. **History** — sličnost s korisnikovom historijom:
   `H(c) = Σ snaga(s) · sim(c, s) / Σ snaga(s)` po svim sjemenima `s`.
   Ako korisnik ima historiju pretrage kategorija, miješa se s interesom za kategoriju:
   `H'(c) = 0.75 · H(c) + 0.25 · min(broj_pretraga_kategorije, 3) / 3`.
   Ako nema nijednog signala, komponenta se izostavlja (cold start).
2. **Proximity** — `1 − min(udaljenost_km, 20) / 20` (haversine); izostavlja se ako lokacija nije poznata.
3. **Quality** — `AvgRating / 5`.
4. **Popularity** — `broj_rezervacija_salona / najveći_broj_rezervacija_među_salonima`.

## 5. Konačni rezultat

Ponderisani prosjek **samo komponenti koje su dostupne** za tog korisnika:

| Komponenta | Težina |
|---|---|
| History | 0.40 |
| Proximity | 0.20 |
| Quality | 0.25 |
| Popularity | 0.15 |

`score = Σ (težina · vrijednost) / Σ težina` (težine dostupnih komponenti se normalizuju).

Vraća se najviše 10 salona, sortiranih po `score` opadajuće.

**Cold start** (novi korisnik bez ijednog signala): koriste se samo Quality, Popularity i (ako postoji lokacija) Proximity, tj. preporuka je popularity-based.

## 6. Objašnjenje preporuke

Svaka preporuka nosi tekst `Reason`, sastavljen samo od signala koji su stvarno doprinijeli:

- sličan salonu X koji ste posjetili / iz vaših favorita / koji ste pregledali (ista kategorija, isti grad, sličan raspon cijena) — kada je sličnost ≥ 0.5,
- pretraživali ste kategoriju Y (Nx),
- već ste ovdje rezervisali Nx / pregledali ste ga Nx,
- u vašim favoritima,
- udaljenost od korisnika (ako je ≥ 0.5 proximity),
- odlična (≥ 4.5) ili dobra (≥ 4.0) ocjena,
- popularan (najmanje 3 rezervacije i ≥ 50% najpopularnijeg salona).

Primjer: `Preporučeno: pretraživali ste kategoriju Hair Salon (2x), već ste ovdje rezervisali 3x, 1.3 km od vas, dobra ocjena 4.2/5.`

Ako nijedan signal nije primjenjiv, prikazuje se: `Preporučeno na osnovu opće popularnosti salona.`

## 7. Konfiguracija

Sve težine i pragovi su imenovane konstante na početku klase `RecommendationService`; nema magičnih brojeva u samom izračunu.

## 8. Performanse

Jedan upit po vrsti signala (grupisanje u bazi po salonu), bez upita unutar petlje. Sličnost i bodovanje računaju se u memoriji nad učitanim salonima.
