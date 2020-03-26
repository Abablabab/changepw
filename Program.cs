using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace changepw {
    class Program {
	
		[DllImport("netapi32.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall,SetLastError=true )]
		static extern uint NetUserChangePassword (
			[MarshalAs(UnmanagedType.LPWStr)] string domainname,
			[MarshalAs(UnmanagedType.LPWStr)] string username,
			[MarshalAs(UnmanagedType.LPWStr)] string oldpassword,
			[MarshalAs(UnmanagedType.LPWStr)] string newpassword
		);
    
		static IDictionary<uint, string> returnValues = new Dictionary<uint, string>() {
			{5, "Access denied"},
			{86, "Invalid current password"},
			{1351, "Cannot access domain"},
			{2221, "User not found"},
			{2226, "PW change must be made on primary domain controller"},
			{2245, "Password too short or simple"},
			{2351, "Invalid computer"}
		};
			
		static void Main(string[] args) {
		
			Console.WriteLine("domain?");
			string domain = Console.ReadLine();
			Console.WriteLine("user?");
			string user = Console.ReadLine();
			Console.WriteLine("current password?");
			string prevPassword = secureRead();
			Console.WriteLine("new password?");
			string newPassword = secureRead();
			
			uint result = NetUserChangePassword(domain, user, prevPassword, newPassword);
			
			if (result == 0) {
				Console.WriteLine("Password changed successfully?");
			} else {
				string errorString;
				try{ 
					errorString = returnValues[result];
				} catch(KeyNotFoundException) {
					errorString = "Unknown error (" + result + ")";
				}
				Console.WriteLine("ERROR: " + errorString);
			}
			
			Console.WriteLine("Press any key to exit");
			Console.ReadKey();
        }
		
		static string secureRead() {
			// I'm not using SecureStrings anywhere because I have to force them 
			// into plaintext to use the windows API. 
			// Not my fault, don't @ me.
			string secret = new string("");
			ConsoleKeyInfo key;
			do {
				key = Console.ReadKey(true);
				
				// If the key is a backspace remove a character from the password
				if (key.Key == ConsoleKey.Backspace && secret.Length > 0) {
					secret = secret.Substring(0, (secret.Length - 1));
					Console.Write("\b \b");
					continue;
				}
				
				// Break out if it's 'Enter'
				if (key.Key == ConsoleKey.Enter) {
					continue;
				}
				
				// Non printable characters will report as this
				if (key.KeyChar != '\u0000')  { 
					secret += key.KeyChar;
					Console.Write("*");
				}
			} while (key.Key != ConsoleKey.Enter);
			Console.WriteLine();
			return secret;
		}
    }
}
