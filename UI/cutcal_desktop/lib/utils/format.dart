import 'package:intl/intl.dart';

const _currencySymbols = {'USD': '\$', 'EUR': '€'};

/// An amount in the currency its salon charges in. With no currency (a total over salons that charge in different ones) it is just the number.
String formatMoney(num amount, String? currency, {int decimals = 2}) {
  final text = amount.toStringAsFixed(decimals);
  if (currency == null) return text;
  final symbol = _currencySymbols[currency];
  return symbol == null ? '$text $currency' : '$symbol$text';
}

/// A time of day the way the salon wants it shown: 24-hour or 12-hour.
String formatTimeOfDay(DateTime time, {required bool is24Hour}) => DateFormat(is24Hour ? 'HH:mm' : 'h:mm a').format(time);

String formatDateTime(DateTime time, {required bool is24Hour, String datePattern = 'MMM d, y'}) =>
    '${DateFormat(datePattern).format(time)} • ${formatTimeOfDay(time, is24Hour: is24Hour)}';
