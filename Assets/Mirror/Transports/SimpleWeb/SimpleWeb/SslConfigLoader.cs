using System.IO;
using System.Security.Authentication;
using UnityEngine;

namespace Mirror.SimpleWeb
{

    public class SslConfigLoader
    {
        internal struct Cert
        {
            public string path;
            public string password;
        }
        public static SslConfig Load(bool sslEnabled, string sslCertJson, SslProtocols sslProtocols)
        {
            // don't need to load anything if ssl is not enabled
            if (!sslEnabled)
                return default;

                 // 新增：如果不是本地文件路径，直接返回
    if (string.IsNullOrEmpty(sslCertJson) || sslCertJson.StartsWith("http"))
    {
        Debug.LogWarning("SSL Cert Json is not a valid local path, skipping load");
        return default;
    }

            string certJsonPath = sslCertJson;

            Cert cert = LoadCertJson(certJsonPath);

            return new SslConfig(
                enabled: sslEnabled,
                sslProtocols: sslProtocols,
                certPath: cert.path,
                certPassword: cert.password
            );
        }

        internal static Cert LoadCertJson(string certJsonPath)
        {
            string json = File.ReadAllText(certJsonPath);
            Cert cert = JsonUtility.FromJson<Cert>(json);

            if (string.IsNullOrWhiteSpace(cert.path))
                throw new InvalidDataException("Cert Json didn't not contain \"path\"");

            // don't use IsNullOrWhiteSpace here because whitespace could be a valid password for a cert
            // password can also be empty
            if (string.IsNullOrEmpty(cert.password))
                cert.password = string.Empty;

            return cert;
        }
    }
}
