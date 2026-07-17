import 'package:flutter_test/flutter_test.dart';

import 'package:cutcal_desktop/main.dart';

void main() {
  testWidgets('App boots to the login screen', (WidgetTester tester) async {
    await tester.pumpWidget(const CutCalDesktopApp());
    await tester.pump();

    expect(find.text('CutCal Admin'), findsWidgets);
  });
}
