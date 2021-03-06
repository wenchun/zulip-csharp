using System.Diagnostics;
using System.IO;

namespace ZulipAPI {

    public class ZulipRCAuth {

        public ZulipAuthentication ZulipAuth { get; private set; }
        public ZulipServer Server { get; private set; }

        public string Username { get; set; }
        public string UserSecret { get; set; }
        public string ServerURL { get; set; }

        public ZulipRCAuth(string ZulipRCPath) {
            LoadZulipRCasString(ZulipRCPath);
        }

        private void LoadZulipRCasString(string ZulipRCPath) {
            string line = "";

            if (File.Exists(ZulipRCPath)) {
                using (var zuliprc = new StreamReader(ZulipRCPath)) {
                    if (ZulipRCIsValid(ZulipRCPath)) {
                        while ((line = zuliprc.ReadLine()) != null) {
                            if (line.Contains("=")) {
                                var KeyValPair = line.Split('=');
                                switch (KeyValPair[0]) {
                                    case "email":
                                        Username = KeyValPair[1];
                                        break;
                                    case "key":
                                        UserSecret = KeyValPair[1];
                                        break;
                                    case "site":
                                        ServerURL = KeyValPair[1];
                                        break;
                                }
                            }
                        }
                    } else {
                        throw new InvalidZulipRCFileException("Invalid .zuliprc file.");
                    }
                }
            } else {
                throw new FileNotFoundException(ZulipRCPath + "could not be found.");
            }

            Server = new ZulipServer(ServerURL);
            ZulipAuth = new ZulipAuthentication(Username, UserSecret);
        }

        private bool ZulipRCIsValid(string ZulipRCPath) {
            using (StreamReader sr = new StreamReader(ZulipRCPath)) {
                string fullZulipRC = sr.ReadToEnd();
                if (fullZulipRC.Contains("email=") &&
                    fullZulipRC.Contains("key=") &&
                    fullZulipRC.Contains("site=")) {
                    return true;
                } else {
                    return false;
                }
            }
        }
    }
}
