using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CutCal.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class UseLocalSeedImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "/images/salons/bellissima-hair-studio-1.jpg");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "/images/salons/bellissima-hair-studio-2.jpg");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "/images/salons/gentlemans-cut-barbershop-1.jpg");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageUrl",
                value: "/images/salons/gentlemans-cut-barbershop-2.jpg");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "/images/salons/glow-beauty-studio-1.jpg");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImageUrl",
                value: "/images/salons/glow-beauty-studio-2.jpg");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImageUrl",
                value: "/images/salons/perfect-nails-studio-1.jpg");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImageUrl",
                value: "/images/salons/perfect-nails-studio-2.jpg");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 9,
                column: "ImageUrl",
                value: "/images/salons/relax-and-spa-1.jpg");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 10,
                column: "ImageUrl",
                value: "/images/salons/relax-and-spa-2.jpg");

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 1,
                column: "ProfileImageUrl",
                value: "/images/salons/bellissima-hair-studio-cover.jpg");

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 2,
                column: "ProfileImageUrl",
                value: "/images/salons/gentlemans-cut-barbershop-cover.jpg");

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 3,
                column: "ProfileImageUrl",
                value: "/images/salons/glow-beauty-studio-cover.jpg");

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 4,
                column: "ProfileImageUrl",
                value: "/images/salons/perfect-nails-studio-cover.jpg");

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 5,
                column: "ProfileImageUrl",
                value: "/images/salons/relax-and-spa-cover.jpg");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 1,
                column: "ProfileImageUrl",
                value: "/images/avatars/11.jpg");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 2,
                column: "ProfileImageUrl",
                value: "/images/avatars/12.jpg");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 3,
                column: "ProfileImageUrl",
                value: "/images/avatars/13.jpg");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 4,
                column: "ProfileImageUrl",
                value: "/images/avatars/14.jpg");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 5,
                column: "ProfileImageUrl",
                value: "/images/avatars/15.jpg");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 6,
                column: "ProfileImageUrl",
                value: "/images/avatars/16.jpg");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 7,
                column: "ProfileImageUrl",
                value: "/images/avatars/17.jpg");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 8,
                column: "ProfileImageUrl",
                value: "/images/avatars/18.jpg");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "ProfileImageUrl",
                value: "/images/avatars/1.jpg");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "ProfileImageUrl",
                value: "/images/avatars/2.jpg");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "ProfileImageUrl",
                value: "/images/avatars/3.jpg");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "ProfileImageUrl",
                value: "/images/avatars/4.jpg");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "ProfileImageUrl",
                value: "/images/avatars/5.jpg");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6,
                column: "ProfileImageUrl",
                value: "/images/avatars/6.jpg");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7,
                column: "ProfileImageUrl",
                value: "/images/avatars/7.jpg");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8,
                column: "ProfileImageUrl",
                value: "/images/avatars/8.jpg");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9,
                column: "ProfileImageUrl",
                value: "/images/avatars/9.jpg");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10,
                column: "ProfileImageUrl",
                value: "/images/avatars/10.jpg");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11,
                column: "ProfileImageUrl",
                value: "/images/avatars/11.jpg");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 12,
                column: "ProfileImageUrl",
                value: "/images/avatars/12.jpg");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 13,
                column: "ProfileImageUrl",
                value: "/images/avatars/13.jpg");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 14,
                column: "ProfileImageUrl",
                value: "/images/avatars/14.jpg");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 15,
                column: "ProfileImageUrl",
                value: "/images/avatars/15.jpg");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 16,
                column: "ProfileImageUrl",
                value: "/images/avatars/16.jpg");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 17,
                column: "ProfileImageUrl",
                value: "/images/avatars/17.jpg");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 18,
                column: "ProfileImageUrl",
                value: "/images/avatars/18.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1562322140-8baeececf3df");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1493256338651-d82f7acb2b38");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1562322140-8baeececf3df");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1493256338651-d82f7acb2b38");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1562322140-8baeececf3df");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1493256338651-d82f7acb2b38");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1562322140-8baeececf3df");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1493256338651-d82f7acb2b38");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 9,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1562322140-8baeececf3df");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 10,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1493256338651-d82f7acb2b38");

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 1,
                column: "ProfileImageUrl",
                value: "https://images.unsplash.com/photo-1560066984-138dadb4c035");

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 2,
                column: "ProfileImageUrl",
                value: "https://images.unsplash.com/photo-1521590832167-7bcbfaa6381f");

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 3,
                column: "ProfileImageUrl",
                value: "https://images.unsplash.com/photo-1522337660859-02fbefca4702");

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 4,
                column: "ProfileImageUrl",
                value: "https://images.unsplash.com/photo-1516975080664-ed2fc6a32937");

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 5,
                column: "ProfileImageUrl",
                value: "https://images.unsplash.com/photo-1585747860715-2ba37e788b70");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 1,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=11");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 2,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=12");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 3,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=13");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 4,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=14");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 5,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=15");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 6,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=16");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 7,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=17");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 8,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=18");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=1");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=2");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=3");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=5");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=6");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=7");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=8");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=9");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=10");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=11");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 12,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=12");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 13,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=13");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 14,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=14");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 15,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=15");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 16,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=16");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 17,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=17");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 18,
                column: "ProfileImageUrl",
                value: "https://i.pravatar.cc/150?img=18");
        }
    }
}
