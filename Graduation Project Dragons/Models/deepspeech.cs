using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;
using System.Diagnostics;
using System.IO;
using DeepSpeechClient.Interfaces;
using DeepSpeechClient;
using DeepSpeechClient.Models;
using Graduation_Project_Dragons.Controllers;

namespace Graduation_Project_Dragons.Models
{
    public class data
    {
        public double start;
        public double end;
        public string word;
        public data()
        {
            start = 0;
            end = 0;
            word = "";
        }

    }
    
    public class deepspeech
    {

        /// <summary>
        /// Get the value of an argurment.
        /// </summary>
        /// <param name="args">Argument list.</param>
        /// <param name="option">Key of the argument.</param>
        /// <returns>Value of the argument.</returns>
        static string GetArgument(IEnumerable<string> args, string option)
        => args.SkipWhile(i => i != option).Skip(1).Take(1).FirstOrDefault();

        static string MetadataToString(CandidateTranscript transcript)
        {
            var nl = Environment.NewLine;
            string retval =
             Environment.NewLine + $"Recognized text: {string.Join("", transcript?.Tokens?.Select(x => x.Text))} {nl}"
             + $"Confidence: {transcript?.Confidence} {nl}"
             + $"Item count: {transcript?.Tokens?.Length} {nl}"
             + string.Join(nl, transcript?.Tokens?.Select(x => $"Timestep : {x.Timestep} TimeOffset: {x.StartTime} Char: {x.Text}"));
            return retval;
        }
        public static void convert_To_Mp3(string x, string m_Path)
        {
            //var snippet = FFmpeg.Conversions.FromSnippet.Convert(x, m_Path);
            //snippet.Start();
        }

        [Obsolete]
        public static string Main1(string s)
        {
            string model = null;
            bool extended = false;
            model = "C:/Users/Nour El-Din/Documents/deepspeech/deepspeech-0.9.3-models.pbmm";
            var scorer = "C:/Users/Nour El-Din/Documents/deepspeech/deepspeech-0.9.3-models.scorer";
            extended = true;
            string wwwPath = HomeController.Environment.WebRootPath;
            string path = Path.Combine(HomeController.Environment.WebRootPath, "Video");
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                Console.WriteLine("Loading model...");
                stopwatch.Start();
                // sphinx-doc: csharp_ref_model_start
                using (IDeepSpeech sttClient = new DeepSpeech(model))
                {
                    // sphinx-doc: csharp_ref_model_stop
                    stopwatch.Stop();
                    Console.WriteLine($"Model loaded - {stopwatch.Elapsed.Milliseconds} ms");
                    stopwatch.Reset();
                    if (scorer != null)
                    {
                        Console.WriteLine("Loading scorer...");
                        sttClient.EnableExternalScorer(scorer);
                    }
                    Directory.SetCurrentDirectory(path);
                    string v_Name =s.Split('.')[0];
                    string x = path+ @$"/{v_Name}.mp4";
                    string m_Path = path+@$"/{v_Name}.mp3";
                    bool mp3_Found = File.Exists(m_Path) ? true : false;
                    string w_Path = path+@$"/{v_Name}.wav";
                    bool wav_Found = File.Exists(w_Path) ? true : false;    
                    string strCmdText;
                    if (!mp3_Found)
                    {
                        strCmdText = $"/c ffmpeg -i {v_Name}.mp4 {v_Name}.mp3";
                        Process process = new Process();
                        process.StartInfo.FileName = "cmd.exe";
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        process.StartInfo.Arguments = strCmdText;
                        process.Start();
                        process.WaitForExit();
                    }
                        
                    if (!wav_Found)
                    {
                        strCmdText = $"/c ffmpeg -i {v_Name}.mp3 -acodec pcm_s16le -ac 1 -ar 16000 {v_Name}.wav";
                        Process process = new Process();
                        process.StartInfo.FileName = "cmd.exe";
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        process.StartInfo.Arguments = strCmdText;
                        process.Start();
                        process.WaitForExit();
                    }
                    string audioFile = path+$"/{v_Name}.wav";
                    var waveBuffer = new WaveBuffer(File.ReadAllBytes(audioFile));
                    using (var waveInfo = new WaveFileReader(audioFile))
                    {
                         Console.WriteLine("Running inference....");
                         stopwatch.Start();
                         string w = "";
                         string speechResult;
                        // sphinx-doc: csharp_ref_inference_start
                         if (extended)
                         { 
                            Metadata metaResult = sttClient.SpeechToTextWithMetadata(waveBuffer.ShortBuffer,
                            Convert.ToUInt32(waveBuffer.MaxSize / 2), 1);
                            speechResult = MetadataToString(metaResult.Transcripts[0]);
                            data new_Data = new data();
                            List<data> full_Text = new List<data>();
                                    double word_Start = 0; ;
                                    double word_End;
                                    string temp = "";
                                    int counter = 0;
                                    int counter2 = 0;
                            int count = 0;
                                    foreach (var c in metaResult.Transcripts[0].Tokens)
                                    {
                                        counter2++;
                                        if (counter == 0)
                                        {
                                            word_Start = c.StartTime;
                                            counter++;
                                        }
                                        temp += c.Text;
                                        if (c.Text == " ")
                                        {
                                            counter = 0;
                                            word_End = c.StartTime;
                                            new_Data.start = word_Start;
                                            new_Data.end = word_End;
                                            new_Data.word = temp;
                                            full_Text.Add(new_Data);
                                            w += temp + ',';
                                            w += (double)word_Start + ",";
                                            w += (double)word_End + "/";
                                            temp = "";
                                            new_Data = new data();

                                            continue;
                                        }
                                        //w += c.Text;
                                        if (counter2 == metaResult.Transcripts[0].Tokens.Length)
                                        {
                                            word_End = c.StartTime;
                                            new_Data.start = word_Start;
                                            new_Data.end = word_End;
                                            new_Data.word = temp;
                                            full_Text.Add(new_Data);
                                            w += temp + ',';
                                            w += (double)word_Start + ",";
                                            w += (double)word_End + "/";
                                            temp = "";
                                        }
                                count++;
                                    }
                         }
                                else
                                {
                                    speechResult = sttClient.SpeechToText(waveBuffer.ShortBuffer,
                                        Convert.ToUInt32(waveBuffer.MaxSize / 2));
                                }
                                // sphinx-doc: csharp_ref_inference_stop
                                stopwatch.Stop();
                                Console.WriteLine($"Audio duration: {waveInfo.TotalTime.ToString()}");
                                Console.WriteLine($"Inference took: {stopwatch.Elapsed.ToString()}");
                        Console.WriteLine((extended ? $"Extended result: " : "Recognized text: ") + speechResult);
                                waveBuffer.Clear();
                                return w;
                            }
                    }
                    
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return " ";
        }
    }
}
