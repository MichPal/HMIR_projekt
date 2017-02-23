using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Hardware;
using Android.Runtime;

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

        TextView _accelerometer_textview;
        TextView _proximity_textview;
        TextView _light_textview;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            FrameLayout _frameLayout = FindViewById<FrameLayout>(Resource.Id.frameLayout1);
            _textureView = FindViewById<TextureView>(Resource.Id.textureView1);
            _textureView.SurfaceTextureListener = this;

            _accelerometer_textview = FindViewById<TextView>(Resource.Id.textView1);
            _proximity_textview = FindViewById<TextView>(Resource.Id.textView2);
            _light_textview = FindViewById<TextView>(Resource.Id.textView3);


            _sensorManager = (SensorManager)GetSystemService(SensorService);

            //------------------------------------ accelerometer ------------------------------------------------------
            _accelerometerSensor = _sensorManager.GetDefaultSensor(SensorType.Accelerometer);
            if (_accelerometerSensor == null)
            {
                _accelerometer_textview.Text = string.Format("Nie je accelerometer!");
            }
            else
            {
                _sensorManager.RegisterListener(this, _accelerometerSensor, Android.Hardware.SensorDelay.Game);
            }
            //------------------------------------ proximity ------------------------------------------------------
            _proximitySensor = _sensorManager.GetDefaultSensor(SensorType.Proximity);
            if (_proximitySensor == null)
            {
                _proximity_textview.Text = string.Format("Nie je proximeter!");
            }
            else
            {
                _sensorManager.RegisterListener(this, _proximitySensor, Android.Hardware.SensorDelay.Game);
            }
            //----------------------------------------- light ----------------------------------------------------    
            _lightSensor = _sensorManager.GetDefaultSensor(SensorType.Light);
            if (_lightSensor == null)
            {
                _light_textview.Text = string.Format("Nie je light sensor!");
            }
            else
            {
                _sensorManager.RegisterListener(this, _lightSensor, Android.Hardware.SensorDelay.Game);
            }



        }

        protected override void OnResume()
        {
            base.OnResume();
          //  _sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Accelerometer), SensorDelay.Ui);
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
            _camera.SetDisplayOrientation(90);
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
                _accelerometer_textview.Text = string.Format("x={0:f}, y={1:f}, z={2:f}", e.Values[0], e.Values[1], e.Values[2]);
            }
            if (e.Sensor.Type == SensorType.Proximity)
            {
                _proximity_textview.Text = string.Format("proximity={0:f}", e.Values[0]);
            }
            if (e.Sensor.Type == SensorType.Light)
            {
                _light_textview.Text = string.Format("Light={0:f}", e.Values[0]);
            }
        }
    }
}

