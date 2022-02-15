// DownloadManager.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace nUpdate
{
    internal static class DownloadManager
    {
        internal static async Task DownloadFile(Uri fileUri, string localFilePath,
            CancellationToken cancellationToken, IProgress<UpdateProgressData> progress)
        {
            long received = 0;
            WebResponse webResponse;
            var webRequest = WebRequest.Create(fileUri);
            using (webResponse = await webRequest.GetResponseAsync())
            {
                long totalSize = webResponse.ContentLength;

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