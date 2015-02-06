using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace GetADaoImage
{
    class Program
    {
        //获取程序当前目录，在目录下创建个Image来存图
        //也就是图片存储的位置
        public static string savePosition = System.Environment.CurrentDirectory + "\\Image";

        static int fail = 0;

        static void Main(string[] args)
        {

            //创建个文件夹存储图，如果文件夹不再就创个
            if (!Directory.Exists(savePosition))
            {
                Directory.CreateDirectory(savePosition);
            }

            //删除目录中的0KB文件
            Delete_0KB();

            log(" ");
            log("----------------" + DateTime.Now + "----------------");
            log(" ");

            int count = 1;


            while (true)
            {
                //当失败过多后退出程序
                //一般失败很多那就是没图下完了，都是404后面
                

                //防止线程过多，限制进程的总线程数不超过50
                while (Process.GetCurrentProcess().Threads.Count > 50)
                {
                    Thread.Sleep(2000);
                }
                
                //启动5个线程下载
                //每个线程启动后休眠半秒钟
                for (int i = 1; i <= 5; i++)
                {
                    
                    ParameterizedThreadStart ParStart = new ParameterizedThreadStart(DownImage);
                    Thread run = new Thread(ParStart);

                    //这里的count.ToString("0000")后面的0000的意思是自动补全到4位
                    run.Start(count.ToString("0000")+".jpg");
                    count++;
                    Thread.Sleep(100);
                }
                if (fail > 30)
                {
                    Thread.Sleep(5000);
                    //删除目录中的0KB文件
                    Delete_0KB();
                    System.Environment.Exit(-1);
                }
                
            }

            

        }


        /// <summary>
        /// 图片下载模块
        /// </summary>
        /// <param name="count">传入图片名</param>
        static void DownImage(object count)
        {
            string name = count.ToString();
            Console.WriteLine(name);
            string imageURL = "http://h.acfunwiki.org/h/";
            WebClient webclient = new WebClient();
            try
            {
                //savePosition + "\\" + name 这个是完整的路径

                
                //如果已经下载了,并且文件大小大于0kb
                if (File.Exists(savePosition + "\\" + name))
                {
                    FileInfo ff = new FileInfo(savePosition + "\\" + name);
                    if (ff.Length<1)
                    {
                        File.Delete(savePosition + "\\" + name);
                        //下载图片
                        webclient.DownloadFile(imageURL + name, savePosition + "\\" + name);
                    }
                }
                else if (!File.Exists(savePosition + "\\" + name))
                {
                    //下载图片
                    webclient.DownloadFile(imageURL + name, savePosition + "\\" + name);
                }

               
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(name+" 获取失败!");
                log(name + " 获取失败!");
                fail++;
                File.Delete(savePosition + "\\" + name);
            }
        }

        //日志模块....
        static void log(string message)
        {
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                sw.WriteLine(message);
                sw.Dispose();
            }
        }


        //删除0KB文件
        static void Delete_0KB()
        {
            DirectoryInfo TheFolder = new DirectoryInfo(savePosition);
            FileInfo[] files = TheFolder.GetFiles();
            foreach (FileInfo c in files)
            {
                if(c.Length<1)
                {
                    c.Delete();
                }
            }
        }
    }
}
