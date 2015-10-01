using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Windows.Resources;

namespace WorkBreakTimer
{	
	enum Status
	{
		nothing, work, play
	}
	
    public partial class MainPage : PhoneApplicationPage
    {
        System.IO.IsolatedStorage.IsolatedStorageSettings appSettings = System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings;
        System.Windows.Threading.DispatcherTimer timer;
		TimeSpan local;
		Status current;
        String message = null, selectedTrack;
        private SoundEffect audioPlay;
		
        public MainPage()
        {
            InitializeComponent();

            // new sound changes to support background audio
            audioPlay = null;

        //    int workhours, workmin, worksec, breakhours, breakmin, breaksec;
        //    string track;
            if (!appSettings.Contains("workhours"))
                appSettings.Add("workhours", 0);
            if (!appSettings.Contains("workmin"))
                appSettings.Add("workmin", 20);
            if (!appSettings.Contains("worksec"))
                appSettings.Add("worksec", 0);
            if (!appSettings.Contains("breakhours"))
                appSettings.Add("breakhours", 0);
            if (!appSettings.Contains("breakmin"))
                appSettings.Add("breakmin", 3);
            if (!appSettings.Contains("breaksec"))
                appSettings.Add("breaksec", 0);
            if (!appSettings.Contains("selectedTrack"))
                appSettings.Add("selectedTrack", "ring2.wav");
            WorkTime.Value = new TimeSpan((int)appSettings["workhours"], (int)appSettings["workmin"], (int)appSettings["worksec"]);
            BreakTime.Value = new TimeSpan((int)appSettings["breakhours"], (int)appSettings["breakmin"], (int)appSettings["breaksec"]);
            selectedTrack = (string)appSettings["selectedTrack"];


			// Addition for multiple tracks of sound
			if(selectedTrack.Equals("ring2.wav"))
				Ring2Radio.IsChecked = true;
			else
				Ring1Radio.IsChecked = true;
			
			try{
                // Holds informations about a file stream.
                StreamResourceInfo SoundFileInfo = App.GetResourceStream(new Uri(selectedTrack, UriKind.Relative));

                // Create the SoundEffect from the Stream
                audioPlay = SoundEffect.FromStream(SoundFileInfo.Stream);
            }
            catch (NullReferenceException) {
                // Display an error message
                MessageBox.Show("Couldn't load sound");
            }
			
			
			
			//BreakTime.Value = new TimeSpan(0,5,0);
			
			 if (Microsoft.Xna.Framework.Media.MediaPlayer.State == MediaState.Playing) {
				//MessageBox.Show("Background audio detected, alarm will ring alongside audio");
				BackgroundAudioMsg.Text = " A playing background audio was detected. The timer alarm will ring alongside this audio. If you do not want that to happen, please close this application";
    		}
			
			WorkTime.Max = new TimeSpan(4,0,0);
			BreakTime.Max = new TimeSpan(0,59,59);
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(timer_Tick);
			current = Status.nothing;
        }
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ResetButton.Visibility = System.Windows.Visibility.Visible;
			if(current == Status.nothing)
			{
                message = "Work!\n";
                local = WorkTime.Value.Value;
                ForOutput.Text = message + local.ToString();
				//ForOutput.Visibility = System.Windows.Visibility.Visible;
        		ForOutputZoomOut.Begin();
				current = Status.work;
			}
			
            if (!this.timer.IsEnabled)
            {
				//audioPlay.Stop();
                this.timer.Start();
                this.button1.Content = "Pause";
            }
            else
            {
				//audioPlay.Stop();
                this.timer.Stop();
                this.button1.Content = "Start";
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            try { FrameworkDispatcher.Update(); } catch { }
			local = local.Subtract(new TimeSpan(0,0,1));
			//if timeout, ring bell
			if(local.Equals(new TimeSpan(0,0,-1)))
			{
				ringbell();
				
                if (current == Status.work)
                {
					//CurrentStatus.Text = "Revise and Rest";
                    local = BreakTime.Value.Value;
					//local = new TimeSpan(0,0,2);
                    current = Status.play;
                    message = "Rest\n";
                }
                else
                {
					//CurrentStatus.Text = "Start your work again!";
                    local = WorkTime.Value.Value;
					//local = new TimeSpan(0,0,4);
                    current = Status.work;
                    message = "Work\n";
                }
			}
            ForOutput.Text = message + local.ToString();
			//CurrentStatus.Text = audioPlay.Source.ToString();
        }

        void ringbell()	//play ringer audio
        {
            //audioPlay.Stop();
            //  audioPlay.Source = new Uri(appSettings["audio"].ToString());
            //  audioPlay.Source = (Uri)appSettings["audio"];
            audioPlay.Play();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            appSettings["workhours"] = WorkTime.Value.Value.Hours;       
            appSettings["workmin"] = WorkTime.Value.Value.Minutes;
            appSettings["worksec"] = WorkTime.Value.Value.Seconds;
            appSettings["breakhours"] = BreakTime.Value.Value.Hours;
            appSettings["breakmin"] = BreakTime.Value.Value.Minutes;
            appSettings["breaksec"] = BreakTime.Value.Value.Seconds;

            base.OnNavigatedTo(e);
        }

        private void Ring1Radio_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
			selectedTrack = "ring1.wav";
			appSettings["selectedTrack"] = selectedTrack;
        	try{
                // Holds informations about a file stream.
                StreamResourceInfo SoundFileInfo = App.GetResourceStream(new Uri(selectedTrack, UriKind.Relative));

                // Create the SoundEffect from the Stream
                audioPlay = SoundEffect.FromStream(SoundFileInfo.Stream);
            }
            catch (NullReferenceException) {
                // Display an error message
                MessageBox.Show("Couldn't load sound");
            }
        }

        private void Ring2Radio_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
        	selectedTrack = "ring2.wav";
			appSettings["selectedTrack"] = selectedTrack;
        	try{
                // Holds informations about a file stream.
                StreamResourceInfo SoundFileInfo = App.GetResourceStream(new Uri(selectedTrack, UriKind.Relative));

                // Create the SoundEffect from the Stream
                audioPlay = SoundEffect.FromStream(SoundFileInfo.Stream);
            }
            catch (NullReferenceException) {
                // Display an error message
                MessageBox.Show("Couldn't load sound");
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (current == Status.nothing)
                return;
			ResetZoomOut.Begin();
            if (timer.IsEnabled)
                timer.Stop();
            current = Status.nothing;
            this.button1.Content = "Start";
            ResetButton.Visibility = System.Windows.Visibility.Collapsed;
        }

    }
}
