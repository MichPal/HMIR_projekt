﻿using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Hardware;
using Android.Runtime;
using Android.Views.Animations;

namespace HMIR_projekt
{
    [Activity(Label = "HMIR_projekt", MainLauncher = true, Icon = "@drawable/Icon")]
    public class MainActivity : Activity, TextureView.ISurfaceTextureListener, Android.Hardware.ISensorEventListener
    {
        // premenne
        Camera _camera;
        TextureView _textureView;
        SensorManager _sensorManager;


        Sensor _proximitySensor;
        Sensor _lightSensor;
        Sensor _accelerometerSensor;

        TextView _accelerometer_textview_x;
        TextView _accelerometer_textview_y;
        TextView _accelerometer_textview_z;
        TextView _light_textview;

        ProgressBar _accelerometer_X;
        ProgressBar _accelerometer_Y;
        ProgressBar _accelerometer_Z;
        ProgressBar _accelerometer_X_reverse;
        ProgressBar _accelerometer_Y_reverse;
        ProgressBar _accelerometer_Z_reverse;

        ImageView _image_strelka;

        Vibrator vibrator;

        bool alert_created = false;
        bool vibrating = false;

        float from_angle = 0.0F;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            FrameLayout _frameLayout = FindViewById<FrameLayout>(Resource.Id.frameLayout1);
            _textureView = FindViewById<TextureView>(Resource.Id.textureView1);
            _textureView.SurfaceTextureListener = this;

            _accelerometer_textview_x = FindViewById<TextView>(Resource.Id.textView1);
            _accelerometer_textview_y = FindViewById<TextView>(Resource.Id.textView2);
            _accelerometer_textview_z = FindViewById<TextView>(Resource.Id.textView3);
            _light_textview = FindViewById<TextView>(Resource.Id.textView4);

            _accelerometer_X = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            _accelerometer_Y = FindViewById<ProgressBar>(Resource.Id.progressBar2);
            _accelerometer_Z = FindViewById<ProgressBar>(Resource.Id.progressBar3);
            _accelerometer_X_reverse = FindViewById<ProgressBar>(Resource.Id.progressBar4);
            _accelerometer_Y_reverse = FindViewById<ProgressBar>(Resource.Id.progressBar5);
            _accelerometer_Z_reverse = FindViewById<ProgressBar>(Resource.Id.progressBar6);

            _image_strelka = FindViewById<ImageView>(Resource.Id.ImageView2);

            vibrator = (Vibrator)this.ApplicationContext.GetSystemService(ContextThemeWrapper.VibratorService);


            _sensorManager = (SensorManager)GetSystemService(SensorService);

            //------------------------------------ accelerometer ------------------------------------------------------
            _accelerometerSensor = _sensorManager.GetDefaultSensor(SensorType.Accelerometer);
            if (_accelerometerSensor == null)
            {
                _accelerometer_textview_x.Text = string.Format("Nie je accelerometer!");
            }
            else
            {
                _sensorManager.RegisterListener(this, _accelerometerSensor, Android.Hardware.SensorDelay.Game);
            }
            //------------------------------------ proximity ------------------------------------------------------
            _proximitySensor = _sensorManager.GetDefaultSensor(SensorType.Proximity);
            if (_proximitySensor == null)
            {
                _accelerometer_textview_y.Text = string.Format("Nie je proximeter!");
            }
            else
            {
                _sensorManager.RegisterListener(this, _proximitySensor, Android.Hardware.SensorDelay.Game);
            }
            //----------------------------------------- light ----------------------------------------------------    
            _lightSensor = _sensorManager.GetDefaultSensor(SensorType.Light);
            if (_lightSensor == null)
            {
                _accelerometer_textview_z.Text = string.Format("Nie je light sensor!");
            }
            else
            {
                _sensorManager.RegisterListener(this, _lightSensor, Android.Hardware.SensorDelay.Game);
            }
        }

    
        

        protected override void OnResume()
        {
            base.OnResume();
            _sensorManager.RegisterListener(this, _accelerometerSensor, SensorDelay.Ui);
            _sensorManager.RegisterListener(this, _proximitySensor, Android.Hardware.SensorDelay.Game);
            _sensorManager.RegisterListener(this, _lightSensor, Android.Hardware.SensorDelay.Game);
        }

        protected override void OnPause()
        {
            base.OnPause();
            _sensorManager.UnregisterListener(this);
        }
        
        //camera preview
        public void OnSurfaceTextureAvailable(Android.Graphics.SurfaceTexture surface, int w, int h)
        {
            _camera = Camera.Open();
            switch (WindowManager.DefaultDisplay.Rotation)
            {
                case SurfaceOrientation.Rotation0:
                    _camera.SetDisplayOrientation(90);
                    break;
                case SurfaceOrientation.Rotation90:
                    _camera.SetDisplayOrientation(0);
                    break;
                case SurfaceOrientation.Rotation270:
                    _camera.SetDisplayOrientation(180);
                    break;
            }
            _textureView.LayoutParameters = new FrameLayout.LayoutParams(w, h);
            try
            {
                _camera.SetPreviewTexture(surface);
                _camera.StartPreview();
            }
            catch (Java.IO.IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public bool OnSurfaceTextureDestroyed(Android.Graphics.SurfaceTexture surface)
        {
            _camera.StopPreview();
            _camera.Release();
            return true;
        }

        public void OnSurfaceTextureSizeChanged(Android.Graphics.SurfaceTexture surface, int width, int height)
        {
            // camera takes care of this
        }

        public void OnSurfaceTextureUpdated(Android.Graphics.SurfaceTexture surface)
        {

        }
                
        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
        }

        public void OnSensorChanged(SensorEvent e)
        {
            if(e.Sensor.Type == SensorType.Accelerometer)
            {
                _accelerometer_textview_x.Text = string.Format("x={0:f}", e.Values[0]);
                _accelerometer_textview_y.Text = string.Format("y={0:f}", e.Values[1]);
                _accelerometer_textview_z.Text = string.Format("z={0:f}", e.Values[2]);

                _accelerometer_X.Progress = (int)((e.Values[0]) * 10);
                _accelerometer_Y.Progress = (int)((e.Values[1]) * 10);
                _accelerometer_Z.Progress = (int)((e.Values[2]) * 10);
                _accelerometer_X_reverse.Progress = (int)((10 + e.Values[0]) * 10);
                _accelerometer_Y_reverse.Progress = (int)((10 + e.Values[1]) * 10);
                _accelerometer_Z_reverse.Progress = (int)((10 + e.Values[2]) * 10);
            }
            if (e.Sensor.Type == SensorType.Proximity)
            {
                if (e.Values[0] == 0 && !alert_created)
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Proximity senzor");
                    alert.SetMessage("Nieco je pred senzorom");
                    alert.SetNeutralButton("OK", (senderAlert, args) =>
                    {
                        alert_created = false;
                    });

                    Dialog dialog = alert.Create();
                    dialog.Show();
                    alert_created = true;
                }
            }
            if (e.Sensor.Type == SensorType.Light)
            {
                 if(e.Values[0] < 400 || e.Values[0] > 14000)
                {
                    _light_textview.SetTextColor(Android.Graphics.Color.Red);
                    _light_textview.Text = string.Format("{0:d} lux", (int)e.Values[0]);

                    if (!vibrating)
                    {
                        vibrating = true;
                        //vibrator.Vibrate(500);
                    }
                    
                }
                else
                {
                    vibrating = false;
                    _light_textview.SetTextColor(Android.Graphics.Color.White);
                    _light_textview.Text = string.Format("{0:d} lux", (int)e.Values[0]);
                }

                float to_Angle = 0.012F * e.Values[0] - 90;
                RotateAnimation rAnim = new RotateAnimation(from_angle, to_Angle, Dimension.RelativeToSelf, 0.5F, Dimension.RelativeToSelf, 0.5F);
                rAnim.Interpolator = new LinearInterpolator();
                rAnim.RepeatCount = Animation.Infinite;
                rAnim.Duration = 100;
                _image_strelka.StartAnimation(rAnim);
                from_angle = to_Angle;
            }
        }

    }
}


