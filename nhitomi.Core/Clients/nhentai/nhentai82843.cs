using System;

namespace nhitomi.Core.Clients.nhentai
{
    public class nhentai82843 : ClientTestCase
    {
        public override string DoujinId => "82843";
        public override Type ClientType => typeof(nhentaiClient);

        public override DoujinInfo KnownValue { get; } = new DoujinInfo
        {
            PrettyName   = "Kami-sama o Chikan",
            OriginalName = "神様を痴漢",
            UploadTime   = DateTime.Parse("2014-06-28T23:14:15+00:00").ToUniversalTime(),
            SourceId     = "82843",
            Parody       = "the world god only knows",
            Characters = new[]
            {
                "keima katsuragi"
            },
            Tags = new[]
            {
                "anal",
                "schoolgirl uniform",
                "glasses",
                "shotacon",
                "yaoi",
                "crossdressing",
                "chikan"
            },
            Artist    = "tomekichi",
            Group     = "tottototomekichi",
            Language  = "japanese",
            PageCount = 26
        };
    }
}