/// The server treats the end of a period as exclusive, so the last day the user picked is sent as the start of the day after it
/// and everything scheduled during that last day is included.
String endOfPeriodParam(DateTime lastDay) => lastDay.add(const Duration(days: 1)).toIso8601String();
