using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication_rpn {
    public static class Utilities {
        public static String getSettingsFilePath() {
            String iniFilePath = null; // System.IO.Path.GetDirectoryName(Environment.CommandLine.Replace("\"", "")) + "\\Stroiproject.ini";
            string exe_file = typeof(ConsoleApplication_rpn.Program).Assembly.Location; // http://stackoverflow.com/questions/4764680/how-to-get-the-location-of-the-dll-currently-executing
            iniFilePath = System.IO.Path.GetDirectoryName(exe_file) + "\\" + System.IO.Path.GetFileNameWithoutExtension(exe_file) + ".settings";
            return iniFilePath;
        }

        public static string getExceptionInfo( Exception ex ) {
            string txt = "\nException:";
            StackTrace st = new StackTrace(ex, true);
            //StackFrame st_frame = st.GetFrame(st.FrameCount - 1);
            for( int i=0; i<=st.FrameCount-1; i++) {
                StackFrame st_frame = st.GetFrame(i);
                txt += "\n"+i+". " + st_frame.GetFileName() + ":(" + st_frame.GetFileLineNumber() + "," + st_frame.GetFileColumnNumber() + ")" + "\n" + ex.Message;
            }
            txt += "\n"+ex.StackTrace;
            return txt;
        }

        public static String getExeFilePath() {
            // http://stackoverflow.com/questions/4764680/how-to-get-the-location-of-the-dll-currently-executing
            String exeFilePath = typeof(ConsoleApplication_rpn.Program).Assembly.Location; // System.IO.Path.GetDirectoryName(Environment.CommandLine.Replace("\"", "")) + "\\Stroiproject.ini";
            return exeFilePath;
        }

        //Extension method for MailMessage to save to a file on disk
        // http://stackoverflow.com/questions/20328598/open-default-mail-client-along-with-a-attachment
        public static void SaveEml( this MailMessage message, string filename, bool addUnsentHeader = true ) {
            using (var filestream = File.Open(filename, FileMode.Create)) {
                if (addUnsentHeader) {
                    var binaryWriter = new BinaryWriter(filestream);
                    //Write the Unsent header to the file so the mail client knows this mail must be presented in "New message" mode
                    binaryWriter.Write(System.Text.Encoding.UTF8.GetBytes("X-Unsent: 1" + Environment.NewLine));
                }

                var assembly = typeof(SmtpClient).Assembly;
                var mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");

                // Get reflection info for MailWriter contructor
                var mailWriterContructor = mailWriterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Stream) }, null);

                // Construct MailWriter object with our FileStream
                var mailWriter = mailWriterContructor.Invoke(new object[] { filestream });

                // Get reflection info for Send() method on MailMessage
                var sendMethod = typeof(MailMessage).GetMethod("Send", BindingFlags.Instance | BindingFlags.NonPublic);

                sendMethod.Invoke(message, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { mailWriter, true, true }, null);

                // Finally get reflection info for Close() method on our MailWriter
                var closeMethod = mailWriter.GetType().GetMethod("Close", BindingFlags.Instance | BindingFlags.NonPublic);

                // Call close method
                closeMethod.Invoke(mailWriter, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { }, null);
            }
        }

    }
}
