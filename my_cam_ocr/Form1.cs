using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using System.Diagnostics;
using System.Windows;
using System.Drawing.Imaging;
using System.Collections;

using utility;
namespace my_cam_ocr
{


    public partial class Form1 : Form
    {
        class VD
        {
            public Bitmap bitmap;
            public TimeSpan timespan;
            public bool isLastSec;
        }
        class VideoOption
        {
            public ArrayList bps = new ArrayList();
            public int V_t = 0;
            public int V_l = 0;
            public int V_w = 0;
            public int V_h = 0;
        }
        static VideoOption videoOption = new VideoOption();
        string version = "0.1"; //版本說明
        string srt_path = "";
        string srt_dir = "";
        double skip_time = 0.5; // half second
        bool isRecording = false;
        private string mn = "";
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = null;
        private static IntPtr _hookID = IntPtr.Zero;
        private static Button r_btn = null;
        private static IntPtr SetHook(LowLevelKeyboardProc proc)

        {

            using (Process curProcess = Process.GetCurrentProcess())

            using (ProcessModule curModule = curProcess.MainModule)

            {

                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,

                    GetModuleHandle(curModule.ModuleName), 0);

            }

        }

        private delegate IntPtr LowLevelKeyboardProc(

            int nCode, IntPtr wParam, IntPtr lParam);

        IntPtr HookCallback(

           int nCode, IntPtr wParam, IntPtr lParam)

        {

            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)

            {

                int vkCode = Marshal.ReadInt32(lParam);

                //Console.WriteLine((Keys)vkCode);
                switch (((Keys)vkCode).ToString())
                {
                    case "F2":
                        {
                            Run_Btn_Click(new Object(), new EventArgs());
                        }
                        break;
                    case "F3":
                        {
                            GoToDir_Btn_Click(new Object(), new EventArgs());
                        }
                        break;
                    case "F8":
                        {
                            HelpMe_Btn_Click(new Object(), new EventArgs());
                        }
                        break;
                }

            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);

        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        private static extern IntPtr SetWindowsHookEx(int idHook,

            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        [return: MarshalAs(UnmanagedType.Bool)]

        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,

            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        private static extern IntPtr GetModuleHandle(string lpModuleName);
        Thread sc = null;
        Thread msc = null;
        static Form2 f2 = null;
        myinclude my = new myinclude();


        static public void show_hide_f2(bool show)
        {
            switch (show)
            {
                case false:
                    f2.lt.Hide();
                    f2.t.Hide();
                    f2.rt.Hide();
                    f2.l.Hide();
                    f2.r.Hide();
                    f2.c.Hide();
                    f2.C_txt.Hide();
                    f2.l.Hide();
                    f2.lb.Hide();
                    f2.b.Hide();
                    f2.rb.Hide();
                    break;
                default:
                    f2.lt.Show();
                    f2.t.Show();
                    f2.rt.Show();
                    f2.l.Show();
                    f2.r.Show();
                    f2.c.Show();
                    f2.C_txt.Show();
                    f2.l.Show();
                    f2.lb.Show();
                    f2.b.Show();
                    f2.rb.Show();
                    break;
            };
        }
        public void thread_msc()
        {
            while(true)
            {
                //Thread.Sleep(1000);
                Thread.Sleep(10);                
                for (int i = 0; i < videoOption.bps.Count; i++)
                {
                    VD vd = (VD)videoOption.bps[i];
                    //vd.bitmap                    
                }
                videoOption.bps.RemoveRange(0, videoOption.bps.Count);
                //GC.Collect();

                if (isRecording==false)
                {

                    //GC.Collect();
                    //write_finish = true;
                    break;
                }
                
            }
          
        }
        public void thread_sc()
        {
            int step_frame = 0;                        
            bool is_first = true;           
            Bitmap bitmap;
            string last_ocr_text = "";
            while (true)
            {
                if (isRecording == false)
                {
                    //write_finish = true;
                    sc.Abort();
                    return;
                }                                                                              
                bitmap = CaptureScreen(true, videoOption.V_l, videoOption.V_t, videoOption.V_w, videoOption.V_h);
                if(bitmap == null)
                {
                    Thread.Sleep(15);                    
                    continue;
                }
                //run image filter
                //image resize
                string output_bitmap_path = my.pwd() + "\\tesseract-ocr\\temp.png";
                                
                bitmap.Save(output_bitmap_path);
                
                //run ocr
                string TESSDATA_PREFIX = my.pwd() + "\\Tesseract-OCR\\tessdata";
                TESSDATA_PREFIX = TESSDATA_PREFIX.Replace("\\", "/");
                string cmd = "chcp 65001 && cd /d " + my.pwd() + "\\tesseract-ocr && set TESSDATA_PREFIX="+ TESSDATA_PREFIX + "&& tesseract.exe temp.png temp -l eng+chi_tra";
                Console.Write(cmd);
                my.system(cmd);


                step_frame++;                
                Thread.Sleep( Convert.ToInt32(skip_time*1000) ); //skip half second
            }
        }

        public Form1()
        {
            InitializeComponent();

        }
        private void alert(string data)
        {
            MessageBox.Show(data);
        }
        private void alert(int data)
        {
            MessageBox.Show(string.Format("{0}", data));
        }
        private void log(string data)
        {
            Console.WriteLine(data);
        }
        private void log(int data)
        {
            Console.WriteLine(data);
        }
        private void log(Size data)
        {
            Console.WriteLine(data.Width + "," + data.Height);
        }

        /* private void run_f2()
         {
             f2 = new Form2();
             f2.InitializeComponent();
             f2.Show();
             f2.TopMost=true;
             f2.Load += new System.EventHandler();


         }
         */

        private void Form1_Load(object sender, EventArgs e)
        {

            //AviManager aviManager = new AviManager(@"E:\5.program\C#\my_cam_old\my_cam\bin\Debug\video\2019-06-23_20_08_40.avi", true);

            //添加音频
            //String fileName = @"E:\5.program\C#\my_cam_old\my_cam\bin\Debug\video\aaa.wav";
            //aviManager.AddAudioStream(fileName, 0);
            //aviManager.Close();

            //AviManager aviManager = new AviManager(@"E:\5.program\C#\my_cam_old\my_cam\bin\Debug\video\2019-06-23_20_41_08.avi", true);
            //aviManager.AddAudioStream(@"E:\5.program\C#\my_cam_old\my_cam\bin\Debug\video\2019-06-23_20_41_08.wav", 0);
            //aviManager.Close();


            _proc = HookCallback;
            r_btn = this.run_btn;
            // From : https://dotblogs.com.tw/huanlin/2008/04/23/3320
            // From : https://dotblogs.com.tw/huanlin/2008/04/23/3319
            _hookID = SetHook(_proc);


            this.Left = Screen.PrimaryScreen.Bounds.Width - this.Width - 120;
            this.Top = Screen.PrimaryScreen.Bounds.Height - this.Height - 120;

            //f2_t = new Thread(run_f2);
            //f2_t.Start();
            f2 = new Form2();
            f2.Show();
            f2.UI_Init();

            srt_dir = my.pwd() + "\\OUTPUT";            
            log(srt_path);
            if (!my.is_dir(srt_dir))
            {
                my.mkdir(srt_dir);
            }
            srt_path = srt_dir + "\\" + my.date("Y-m-d_H_i_s")+".srt";




        }
        public void renew_path()
        {
            srt_path = srt_dir + "\\" + my.date("Y-m-d_H_i_s") + ".srt";
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("你確定要離開嗎.", "離開", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dr == DialogResult.Yes)
            {
                // Do something 
                if (sc != null)
                {
                    sc.Abort();
                }
                GC.Collect();
                //Application.Exit();
            }
            else
            {
                e.Cancel = true;
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public POINTAPI ptScreenPos;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct POINTAPI
        {
            public int x;
            public int y;
        }
        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);
        [DllImport("user32.dll")]
        static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

        const Int32 CURSOR_SHOWING = 0x00000001;
        public static Bitmap CaptureScreen(bool CaptureMouse, int l, int t, int w, int h)
        {
            //Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height

            Bitmap result = new Bitmap(w, h, PixelFormat.Format24bppRgb);
            try
            {
                
                using (Graphics g = Graphics.FromImage(result))
                {
                    //Screen.PrimaryScreen.Bounds.Size
                    g.CopyFromScreen(l, t, 0, 0, new Size(w, h), CopyPixelOperation.SourceCopy);

                    if (CaptureMouse)
                    {
                        CURSORINFO pci;
                        pci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CURSORINFO));

                        if (GetCursorInfo(out pci))
                        {
                            if (pci.flags == CURSOR_SHOWING)
                            {
                                DrawIcon(g.GetHdc(), pci.ptScreenPos.x - l, pci.ptScreenPos.y - t, pci.hCursor);
                                g.ReleaseHdc();
                            }
                        }
                    }
                }
            }
            catch
            {
                result = null;
            }
            //myinclude my = new myinclude();
            //result.Save("C:\\temp\\a.bmp");
            //Application.Exit();
            return result;
        }
        public void Run_Btn_Click(object sender, EventArgs e)
        {
            switch (run_btn.Text)
            {
                case "開始辨識 (F2)":                    
                    renew_path();
                    isRecording = true;
                    run_btn.Text = "停止辨識 (F2)";
                    f2.can_drag = false;

                    videoOption.V_l = f2.Left;
                    videoOption.V_t = f2.Top;
                    videoOption.V_w = f2.Width;
                    videoOption.V_h = f2.Height;


                    int width = videoOption.V_w;
                    int height = videoOption.V_h;
                    //from https://en.code-bude.net/2013/04/17/how-to-create-video-files-in-c-from-single-images/
                    /*my.file_put_contents(video_path, "");*/

                    //writer.Open("C:\\temp\\a.avi", V_w, V_h);
                    show_hide_f2(false);
                    int w = Convert.ToInt32(Math.Ceiling(videoOption.V_w / 10.0)) * 10;
                    int h = Convert.ToInt32(Math.Ceiling(videoOption.V_h / 10.0)) * 10;

                    timer1.Enabled = true;
                    sc = new Thread(thread_sc);
                    sc.Start();



                    break;
                default:
                    isRecording = false;
                    show_hide_f2(true);
                    run_btn.Text = "開始辨識 (F2)";
                    f2.can_drag = true;
                    break;
            }
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)//縮小時
            {
                this.notifyIcon1.Visible = true;//顯示Icon
                this.Hide();//隱藏Form
            }
            else//放大時
            {
            }
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();//顯示Form
            this.WindowState = FormWindowState.Normal;//回到正常大小
            this.Activate();//焦點
            this.Focus();//焦點
        }

        private void HelpMe_Btn_Click(object sender, EventArgs e)
        {
            string message = @"
程式名稱：螢幕文字辨識
開發者：羽山 (http://3wa.tw)
版本：" + version + @"
程式說明：
　　1、框好你想辨識的螢幕範圍
　　2、按下「F2」就開始辨識
　　3、再按一次「F2」就結束
　　4、辨識好的文字，會放在本程式 OUTPUT 目錄下
";
            alert(message);
        }

        private void GoToDir_Btn_Click(object sender, EventArgs e)
        {
            string path = my.pwd() + "\\OUTPUT";
            my.system("explorer.exe \"" + path + "\"");
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            GC.Collect();
        }
    }
}
