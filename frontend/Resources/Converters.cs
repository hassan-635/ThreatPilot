using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ThreatPilot.Frontend.Resources
{
    public class SeverityToBgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value?.ToString()?.ToUpper()) switch
            {
                "CRITICAL" => new SolidColorBrush(Color.FromRgb(254, 226, 226)),
                "HIGH" => new SolidColorBrush(Color.FromRgb(255, 247, 237)),
                "MEDIUM" => new SolidColorBrush(Color.FromRgb(255, 251, 235)),
                "LOW" => new SolidColorBrush(Color.FromRgb(240, 253, 244)),
                _ => new SolidColorBrush(Color.FromRgb(241, 245, 249))
            };
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class SeverityToFgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value?.ToString()?.ToUpper()) switch
            {
                "CRITICAL" => new SolidColorBrush(Color.FromRgb(220, 38, 38)),
                "HIGH" => new SolidColorBrush(Color.FromRgb(234, 88, 12)),
                "MEDIUM" => new SolidColorBrush(Color.FromRgb(217, 119, 6)),
                "LOW" => new SolidColorBrush(Color.FromRgb(22, 163, 74)),
                _ => new SolidColorBrush(Color.FromRgb(100, 116, 139))
            };
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class StatusToBgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value?.ToString()) switch
            {
                "Open" => new SolidColorBrush(Color.FromRgb(239, 246, 255)),
                "Investigating" => new SolidColorBrush(Color.FromRgb(255, 247, 237)),
                "Resolved" => new SolidColorBrush(Color.FromRgb(240, 253, 244)),
                _ => new SolidColorBrush(Color.FromRgb(241, 245, 249))
            };
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class StatusToFgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value?.ToString()) switch
            {
                "Open" => new SolidColorBrush(Color.FromRgb(37, 99, 235)),
                "Investigating" => new SolidColorBrush(Color.FromRgb(234, 88, 12)),
                "Resolved" => new SolidColorBrush(Color.FromRgb(22, 163, 74)),
                _ => new SolidColorBrush(Color.FromRgb(100, 116, 139))
            };
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class TimeAgoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
            {
                var diff = DateTime.Now - dt;
                if (diff.TotalMinutes < 1) return "Just now";
                if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}m ago";
                if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h ago";
                return $"{(int)diff.TotalDays}d ago";
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b && b ? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
