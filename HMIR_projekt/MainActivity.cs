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
    public class MainActivity : Activity, TextureView.ISurfaceTextureListener, ISensorEventListener 
    {
        // premenne
        Camera _camera;
        TextureView _textureView;
        SensorManager _sensorManager;
        TextView _accelerometer_textview;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            FrameLayout _frameLayout = FindViewById<FrameLayout>(Resource.Id.frameLayout1);
            _textureView = FindViewById<TextureView>(Resource.Id.textureView1);
            _textureView.SurfaceTextureListener = this;

            _sensorManager = (SensorManager)GetSystemService(SensorService);
            _accelerometer_textview = FindViewById<TextView>(Resource.Id.textView1);

        }

        protected override void OnResume()
        {
            base.OnResume();
            _sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Accelerometer), SensorDelay.Ui);
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
            _accelerometer_textview.Text = string.Format("x={0:f}, y={1:f}, z={2:f}", e.Values[0], e.Values[1], e.Values[2]);
        }
    }
}

