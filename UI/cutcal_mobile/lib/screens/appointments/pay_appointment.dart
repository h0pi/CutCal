import 'package:flutter/material.dart';

import '../../models/models.dart';
import 'paypal_webview_screen.dart';
import 'stripe_checkout_screen.dart';

/// Pays an unpaid appointment with the method it was booked with; true once it is paid, false if the customer backed out.
Future<bool> payAppointment(BuildContext context, AppointmentModel appointment, TipChoice tip) => appointment.paymentMethod == 'Stripe'
    ? payWithStripe(context, appointment.id, tip)
    : payWithPayPal(context, appointment.id, tip);
