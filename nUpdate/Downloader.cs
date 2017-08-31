// Copyright © Dominic Beger 2017

using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace nUpdate
{
    internal static class Downloader
    {
        internal static async Task DownloadFile(Uri fileUri, string localFilePath, long totalSize,
            CancellationToken cancellationToken, IProgress<UpdateProgressData> progress)
        {
            long received = 0;
            WebResponse webResponse;
            var webRequest = WebRequest.Create(fileUri);
            using (webResponse = await webRequest.GetResponseAsync())
            {
                var buffer = new byte[1024];
                using (var fileStream = File.Create(localFilePath))
                {
                    using (var input = webResponse.GetResponseStream())
                    {
                        if (input == null)
                            throw new Exception("The response stream couldn't be read.");

                        var size = await input.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        while (size > 0) // As long as we receive bytes from the stream...
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await fileStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
                            received += size;
                            progress?.Report(new UpdateProgressData(received,
                                // ReSharper disable once PossibleLossOfFraction
                                totalSize, (float) (received / totalSize) * 100));
                            size = await input.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        }
                    }
                }
            }
        }
    }
}