using MemApp.Application.Interfaces;
using ResApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.Services;


public class PasswordNewHash : IPasswordNewHash
{
    public const int SALT_BYTE_SIZE = 24;
    public const int HASH_BYTE_SIZE = 24;
    public const int PBKDF2_ITERATIONS = 1000;
    public const int ITERATION_INDEX = 0;
    public const int SALT_INDEX = 1;
    public const int PBKDF2_INDEX = 2;

    public string CreateHash(string password, ref string passwordHash, ref string passwordSalt)
    {
        if (passwordHash == null) throw new ArgumentNullException("passwordHash");
        if (passwordSalt == null) throw new ArgumentNullException("passwordSalt");
        // Generate a random salt
        // RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
        byte[] salt = new byte[SALT_BYTE_SIZE];
        RandomNumberGenerator.Fill(salt);
        // csprng.GetBytes(salt);

        // Hash the password and encode the parameters
        byte[] hash = PBKDF2(password, salt, PBKDF2_ITERATIONS, HASH_BYTE_SIZE);

        passwordHash = Convert.ToBase64String(hash);
        passwordSalt = Convert.ToBase64String(salt);

        return PBKDF2_ITERATIONS + ":" +
            Convert.ToBase64String(salt) + ":" +
            Convert.ToBase64String(hash);
    }
    public bool ValidatePassword(string password, string passwordHash, string passwordSalt)
    {

        // Extract the parameters from the hash
        char[] delimiter = { ':' };
        //string[] split = correctHash.Split(delimiter);
        int iterations = PBKDF2_ITERATIONS;//Int32.Parse(split[ITERATION_INDEX]);
        byte[] salt = Convert.FromBase64String(passwordSalt);
        byte[] hash = Convert.FromBase64String(passwordHash);

        byte[] testHash = PBKDF2(password, salt, iterations, hash.Length);
        return SlowEquals(hash, testHash);
    }
    private bool SlowEquals(byte[] a, byte[] b)
    {
        uint diff = (uint)a.Length ^ (uint)b.Length;
        for (int i = 0; i < a.Length && i < b.Length; i++)
            diff |= (uint)(a[i] ^ b[i]);
        return diff == 0;
    }
    private byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes)
    {
       

        using var pbkdf2 = new Rfc2898DeriveBytes(
        password,
        salt,
        iterations,
        HashAlgorithmName.SHA256); // You can use SHA1, SHA256, or SHA512

        return pbkdf2.GetBytes(outputBytes);
    }

    public string GetEncryptedPassword(string password)
    {
       
        string returnValue = Convert.ToBase64String(Encoding.ASCII.GetBytes(password));

        // Create MD5 hash
        using MD5 md5 = MD5.Create();
        byte[] inputBytes = Encoding.Default.GetBytes(returnValue); // Or Encoding.UTF8
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        // Convert hash to hex string
        StringBuilder sb = new StringBuilder();
        foreach (byte b in hashBytes)
        {
            sb.Append(b.ToString("x2".ToLower())); // hex format
        }

        return sb.ToString();
    }
}
