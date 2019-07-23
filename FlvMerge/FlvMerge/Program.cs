using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlvMerge
{
    class Program
    {
        static readonly bool metadataOnly = false;

        static void Main(string[] args)
        {
            string description = "description";
            string metadatacreator = "metadatacreator";
            bool hasKeyframes = false;
            bool hasVideo = false;
            bool hasAudio = false;
            bool hasMetadata = false;
            bool canSeekToEnd = false;
            double duration = 0;
            uint datasize = 0;
            uint videosize = 0;
            double framerate = 0;
            double videodatarate = 0;
            double videocodecid = 0;
            double width = 0;
            double height = 0;
            uint audiosize = 0;
            double audiodatarate = 0;
            double audiocodecid = 0;
            double audiosamplerate = 0;
            double audiosamplesize = 0;
            bool stereo = false;
            double filesize = 0;
            uint lasttimestamp = 0;
            uint lastkeyframetimestamp = 0;
            uint lastkeyframelocation = 0;
            List<uint> keyframesFilepositions = new List<uint>();
            List<double> keyframesTimes = new List<double>();

            using (FlvFile flv = new FlvFile("1.flv"))
            {
                using (FileStream stream = new FileStream("2.flv", FileMode.Create))
                {
                    stream.Write(flv.Header.HeaderBytes, 0, flv.Header.HeaderBytes.Length);
                    do
                    {
                        try
                        {
                            FlvFile.Tag tag = flv.ReadTag();
                            if (tag.Type == FlvFile.Tag.TagType.Script)
                            {
                                FlvFile.Tag.ScriptTag scriptTag = (FlvFile.Tag.ScriptTag)tag;
                                FlvFile.Tag.ScriptTag.String tagName = (FlvFile.Tag.ScriptTag.String)scriptTag.Name;
                                if (tagName.Value == "onMetaData")
                                {
                                    FlvFile.Tag.ScriptTag.EcmaArray tagValue = (FlvFile.Tag.ScriptTag.EcmaArray)scriptTag.Value;
                                    hasKeyframes |= ((FlvFile.Tag.ScriptTag.Boolean)tagValue.Items["hasKeyframes"]).Value;
                                    hasVideo |= ((FlvFile.Tag.ScriptTag.Boolean)tagValue.Items["hasVideo"]).Value;
                                    hasAudio |= ((FlvFile.Tag.ScriptTag.Boolean)tagValue.Items["hasAudio"]).Value;
                                    hasMetadata |= ((FlvFile.Tag.ScriptTag.Boolean)tagValue.Items["hasMetadata"]).Value;
                                    canSeekToEnd |= ((FlvFile.Tag.ScriptTag.Boolean)tagValue.Items["canSeekToEnd"]).Value;
                                    duration += ((FlvFile.Tag.ScriptTag.Number)tagValue.Items["duration"]).Value;
                                    videocodecid = ((FlvFile.Tag.ScriptTag.Number)tagValue.Items["videocodecid"]).Value;
                                    width = ((FlvFile.Tag.ScriptTag.Number)tagValue.Items["width"]).Value;
                                    height = ((FlvFile.Tag.ScriptTag.Number)tagValue.Items["height"]).Value;
                                    audiocodecid = ((FlvFile.Tag.ScriptTag.Number)tagValue.Items["audiocodecid"]).Value;
                                    audiosamplerate = ((FlvFile.Tag.ScriptTag.Number)tagValue.Items["audiosamplerate"]).Value;
                                    audiosamplesize = ((FlvFile.Tag.ScriptTag.Number)tagValue.Items["audiosamplesize"]).Value;
                                    stereo |= ((FlvFile.Tag.ScriptTag.Boolean)tagValue.Items["stereo"]).Value;
                                }
                            }
                            else
                            {
                                datasize += tag.TagLengthWithPts;
                                lasttimestamp = tag.Header.Timestamp;
                                if (tag.Type == FlvFile.Tag.TagType.Video)
                                {
                                    FlvFile.Tag.VideoTag videoTag = (FlvFile.Tag.VideoTag)tag;
                                    videosize += videoTag.TagLength;
                                    framerate++;
                                    videodatarate += videoTag.BodyLength;
                                    if (videoTag.FrameType == FlvFile.Tag.VideoTag.FrameTypes.KeyFrame)
                                    {
                                        lastkeyframetimestamp = videoTag.Header.Timestamp;
                                        lastkeyframelocation = (uint)stream.Position;
                                        keyframesFilepositions.Add((uint)stream.Position);
                                        keyframesTimes.Add((double)videoTag.Header.Timestamp/1000);
                                    }
                                }
                                else if (tag.Type == FlvFile.Tag.TagType.Audio)
                                {
                                    FlvFile.Tag.AudioTag audioTag = (FlvFile.Tag.AudioTag)tag;
                                    audiosize += audioTag.TagLength;
                                    audiodatarate += audioTag.BodyLength;
                                }
                            }
                            byte[] buffer = tag.TagBytesWithPts;
                            stream.Write(buffer, 0, buffer.Length);
                            filesize += buffer.Length;
                        }
                        catch (FlvFile.EofException)
                        {
                            break;
                        }
                    }
                    while (!metadataOnly);
                }
            }

            framerate /= duration;
            videodatarate /= duration * 1024 / 8;
            audiodatarate /= duration * 1024 / 8;
        }
    }
}
