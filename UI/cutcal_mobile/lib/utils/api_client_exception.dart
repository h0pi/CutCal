class ApiClientException implements Exception {
  final String message;
  final int? statusCode;

  ApiClientException(this.message, {this.statusCode});

  @override
  String toString() => message;
}
