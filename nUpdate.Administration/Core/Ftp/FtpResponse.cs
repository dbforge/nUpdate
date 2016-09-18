// Author: Dominic Beger (Trade/ProgTrade) 2016

namespace nUpdate.Administration.Core.Ftp
{
    /// <summary>
    ///     FTP response class containing the FTP raw text, response code, and other information.
    /// </summary>
    public class FtpResponse
    {
        /// <summary>
        ///     Default constructor for FtpResponse.
        /// </summary>
        public FtpResponse()
        {
        }

        /// <summary>
        ///     Constructor for FtpResponse.
        /// </summary>
        /// <param name="rawText">Raw text information sent from the FTP server.</param>
        public FtpResponse(string rawText)
        {
            RawText = rawText;
            Text = ParseText(rawText);
            Code = ParseCode(rawText);
            IsInformational = ParseInformational(rawText);
        }

        /// <summary>
        ///     Constructor for FtpResponse.
        /// </summary>
        /// <param name="response">FtpResponse object.</param>
        public FtpResponse(FtpResponse response)
        {
            RawText = response.RawText;
            Text = response.Text;
            Code = response.Code;
            IsInformational = response.IsInformational;
        }

        /// <summary>
        ///     Get raw server response text information.
        /// </summary>
        public string RawText { get; }

        /// <summary>
        ///     Get the server response text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        ///     Get a value indicating the FTP server response code.
        /// </summary>
        public FtpResponseCode Code { get; } = FtpResponseCode.None;

        internal bool IsInformational { get; }

        private FtpResponseCode ParseCode(string rawText)
        {
            FtpResponseCode code = FtpResponseCode.None;

            if (rawText.Length >= 3)
            {
                string codeString = rawText.Substring(0, 3);
                int codeInt = 0;

                if (int.TryParse(codeString, out codeInt))
                {
                    code = (FtpResponseCode) codeInt;
                }
            }

            return code;
        }

        private string ParseText(string rawText)
        {
            if (rawText.Length > 4)
                return rawText.Substring(4).Trim();
            return string.Empty;
        }

        private bool ParseInformational(string rawText)
        {
            if (rawText.Length >= 4 && rawText[3] == '-')
                return true;
            return false;
        }
    }
}