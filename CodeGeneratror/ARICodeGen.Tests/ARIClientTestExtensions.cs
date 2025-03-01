using System.Threading.Tasks;
using Arke.ARI;

namespace ARICodeGen.Tests
{
    /// <summary>
    /// Extensions for testing the ARIClient
    /// </summary>
    public static class ARIClientTestExtensions
    {
        /// <summary>
        /// Test helper method to invoke GetAsync on the client with a specific endpoint
        /// </summary>
        public static Task<T> GetTestAsync<T>(this ARIClient client, string endpoint)
        {
            // We can now call the protected internal method directly
            return client.GetAsync<T>(endpoint);
        }

        /// <summary>
        /// Test helper method to invoke PostAsync on the client with a specific endpoint and data
        /// </summary>
        public static Task<T> PostTestAsync<T>(this ARIClient client, string endpoint, object data)
        {
            // We can now call the protected internal method directly
            return client.PostAsync<T>(endpoint, data);
        }
    }
} 