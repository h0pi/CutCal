import 'package:cutcal_desktop/providers/entity_providers.dart';
import 'package:cutcal_desktop/utils/api_client_exception.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;

void main() {
  final provider = ReportProvider();

  test('a PDF response is returned as bytes', () {
    final response = http.Response('%PDF-1.4', 200, headers: {'content-type': 'application/pdf'});

    expect(provider.validatePdfResponse(response), response.bodyBytes);
  });

  test('an error response throws the message of the server instead of being saved as a PDF', () {
    final response = http.Response('{"message":"You are not allowed to view this report."}', 403, headers: {'content-type': 'application/json'});

    expect(
      () => provider.validatePdfResponse(response),
      throwsA(isA<ApiClientException>().having((e) => e.message, 'message', 'You are not allowed to view this report.')),
    );
  });

  test('a successful response that is not a PDF throws', () {
    final response = http.Response('{"a":1}', 200, headers: {'content-type': 'application/json'});

    expect(() => provider.validatePdfResponse(response), throwsA(isA<ApiClientException>()));
  });
}
