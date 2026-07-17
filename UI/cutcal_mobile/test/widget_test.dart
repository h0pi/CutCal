import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'package:cutcal_mobile/main.dart';

void main() {
  testWidgets('App boots to the login screen', (WidgetTester tester) async {
    await tester.pumpWidget(const CutCalApp());
    await tester.pump();

    expect(find.text('CutCal'), findsWidgets);
    expect(find.widgetWithText(ElevatedButton, 'Log in'), findsNothing);
  });
}
