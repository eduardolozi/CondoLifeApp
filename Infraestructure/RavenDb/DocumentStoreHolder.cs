using Domain.Models;
using Raven.Client.Documents;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using static Raven.Client.Constants;

namespace Infraestructure.RavenDb {
    public class DocumentStoreHolder {
        private static Lazy<IDocumentStore> store = new Lazy<IDocumentStore>(CreateStore);

        public static IDocumentStore Store => store.Value;

        private static IDocumentStore CreateStore() {
            IDocumentStore store = new DocumentStore() {
                Urls = ["https://localhost"],

                Conventions =
                {
                    MaxNumberOfRequestsPerSession = 10,
                    UseOptimisticConcurrency = true
                },
                Database = "Condolife",

                Certificate = new X509Certificate2("C:\\Users\\eduardo.azevedo\\Documents\\volumes\\ravendb\\certs\\my-test.instance.pfx"),

            }.Initialize();

            return store;
        }
    }
}
