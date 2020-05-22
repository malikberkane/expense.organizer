using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Text.Method;
using Android.Widget;
using expense.manager.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using expense.manager.Views;
[assembly: ExportRenderer(typeof(NumericInput), typeof(NumericInputRenderer))]

namespace expense.manager.Droid
{

        public class NumericInputRenderer : EntryRenderer
        {
            public NumericInputRenderer(Context context) : base(context)
            {

            }

            private EditText _native = null;

            protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
            {
                base.OnElementChanged(e);

                if (e.NewElement == null)
                    return;

                _native = Control as EditText;
                _native.InputType = Android.Text.InputTypes.ClassNumber;
                if ((e.NewElement as NumericInput).AllowNegative == true)
                    _native.InputType |= InputTypes.NumberFlagSigned;
                if ((e.NewElement as NumericInput).AllowFraction == true)
                {
                    _native.InputType |= InputTypes.NumberFlagDecimal;
                    _native.KeyListener = DigitsKeyListener.GetInstance(string.Format("1234567890{0}", "."));
                }
                if (e.NewElement.FontFamily != null)
                {
                    var font = Typeface.CreateFromAsset(Android.App.Application.Context.Assets, e.NewElement.FontFamily);
                    _native.Typeface = font;
                }
            }

            protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                base.OnElementPropertyChanged(sender, e);
                if (_native == null)
                    return;

                if (e.PropertyName == NumericInput.AllowNegativeProperty.PropertyName)
                {
                    if ((sender as NumericInput).AllowNegative == true)
                    {
                        // Add Signed flag
                        _native.InputType |= InputTypes.NumberFlagSigned;
                    }
                    else
                    {
                        // Remove Signed flag
                        _native.InputType &= ~InputTypes.NumberFlagSigned;
                    }
                }
                if (e.PropertyName == NumericInput.AllowFractionProperty.PropertyName)
                {
                    if ((sender as NumericInput).AllowFraction == true)
                    {
                        // Add Decimal flag
                        _native.InputType |= InputTypes.NumberFlagDecimal;
                        _native.KeyListener = DigitsKeyListener.GetInstance(string.Format("1234567890{0}", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                    }
                    else
                    {
                        // Remove Decimal flag
                        _native.InputType &= ~InputTypes.NumberFlagDecimal;
                        _native.KeyListener = DigitsKeyListener.GetInstance(string.Format("1234567890"));
                    }
                }
            }
        }
    }
