import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../providers/auth_provider.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/utils_widgets.dart';

class RegisterScreen extends StatefulWidget {
  const RegisterScreen({super.key});

  @override
  State<RegisterScreen> createState() => _RegisterScreenState();
}

class _RegisterScreenState extends State<RegisterScreen> {
  final _formKey = GlobalKey<FormState>();
  final _username = TextEditingController();
  final _firstName = TextEditingController();
  final _lastName = TextEditingController();
  final _email = TextEditingController();
  final _phone = TextEditingController();
  final _password = TextEditingController();
  final _confirmPassword = TextEditingController();
  bool _isLoading = false;

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() => _isLoading = true);
    try {
      await context.read<AuthProvider>().register({
        'username': _username.text,
        'firstName': _firstName.text,
        'lastName': _lastName.text,
        'email': _email.text,
        'phone': _phone.text,
        'password': _password.text,
        'confirmPassword': _confirmPassword.text,
      });
      if (mounted) {
        showSuccessSnackBar(context, 'Account created. You can now log in.');
        Navigator.of(context).pop();
      }
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Create account')),
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(24),
          child: Form(
            key: _formKey,
            child: Column(
              children: [
                _field(_username, 'Username'),
                _field(_firstName, 'First name'),
                _field(_lastName, 'Last name'),
                _field(_email, 'Email', validator: (v) => v != null && v.contains('@') ? null : 'Invalid email'),
                _field(_phone, 'Phone', required: false),
                _field(_password, 'Password', obscure: true, validator: (v) => (v?.length ?? 0) >= 6 ? null : 'Minimum 6 characters'),
                _field(
                  _confirmPassword,
                  'Confirm password',
                  obscure: true,
                  validator: (v) => v == _password.text ? null : 'Passwords do not match',
                ),
                const SizedBox(height: 24),
                SizedBox(
                  width: double.infinity,
                  child: FilledButton(
                    onPressed: _isLoading ? null : _submit,
                    child: _isLoading
                        ? const SizedBox(height: 20, width: 20, child: CircularProgressIndicator(strokeWidth: 2))
                        : const Text('Register'),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _field(TextEditingController controller, String label,
      {bool obscure = false, bool required = true, String? Function(String?)? validator}) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 16),
      child: TextFormField(
        controller: controller,
        obscureText: obscure,
        decoration: InputDecoration(labelText: label, border: const OutlineInputBorder()),
        validator: validator ?? (required ? (v) => (v == null || v.isEmpty) ? '$label is required' : null : null),
      ),
    );
  }
}
