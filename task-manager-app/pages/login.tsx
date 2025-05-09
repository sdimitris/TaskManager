'use client';

import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { useRouter } from 'next/router';

type LoginFormInputs = {
  username: string;
  password: string;
};

export default function Login() {
  const { register, handleSubmit, formState: { errors } } = useForm<LoginFormInputs>();
  const [loading, setLoading] = useState(false);
  const [loginError, setLoginError] = useState<string | null>(null);
  const router = useRouter();

  const onSubmit = async (data: LoginFormInputs) => {
    setLoading(true);
    setLoginError(null);

    try {
      // Replace this with your login API
      const response = await fetch('http://localhost:8080/api/User/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });

      if (!response.ok) {
        const err = await response.json();
        setLoginError(err.message || 'Login failed');
      } else {
        const result  = await response.json();
        localStorage.setItem('token', result.token); // Save token
        router.push('/home'); // Redirect to home page
      }
    } catch (error) {
      setLoginError('An unexpected error occurred.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-sm mx-auto mt-10 p-6 bg-white shadow-xl rounded-2xl">
      <h2 className="text-black text-2xl font-bold mb-6 text-center">Login</h2>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div>
          <label className="text-black block mb-1 font-medium">Username</label>
          <input
            type="text"
            {...register('username', { required: 'Username is required' })}
            className="text-black w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2"
          />
          {errors.username && <p className="text-red-500 text-sm">{errors.username.message}</p>}s
        </div>

        <div>
          <label className="text-black block mb-1 font-medium">Password</label>
          <input
            type="password"
            {...register('password', { required: 'Password is required' })}
            className="text-black w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2"
          />
          {errors.password && <p className="text-red-500 text-sm">{errors.password.message}</p>}
        </div>

        {loginError && <p className="text-red-600 text-sm">{loginError}</p>}

        <button
          type="submit"
          disabled={loading}
          className="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition"
        >
          {loading ? 'Logging in...' : 'Login'}
        </button>
      </form>
    </div>
  );
}
