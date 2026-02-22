using CodeClinic;
using LiveCharts;
using LiveCharts.Configurations;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Dashboard
{
    /// <summary>
    /// Interaction logic for ConstantChangesChart.xaml
    /// </summary>
    public partial class ConstantChangesChart : UserControl, INotifyPropertyChanged
    {
        private static long tickZero = DateTime.Parse("2018-01-01T08:00:00Z").Ticks;

        public Func<double, string> X_Axis_LabelFormatter { get; set; } = d => TimeSpan.FromTicks((long)d - tickZero).TotalSeconds.ToString();

        public ConstantChangesChart()
        {
            InitializeComponent();

            lsEfficiency.Configuration = Mappers.Xy<FactoryTelemetry>().X(ft => ft.TimeStamp.Ticks).Y(ft => ft.Efficiency);

            lsPulse.Configuration = Mappers.Xy<FactoryTelemetry>().X(ft => ft.TimeStamp.Ticks).Y(ft => ft.Pulse);

            lsRed.Configuration = Mappers.Xy<FactoryTelemetry>().X(ft => ft.TimeStamp.Ticks).Y(ft => ft.Red);
            lsGreen.Configuration = Mappers.Xy<FactoryTelemetry>().X(ft => ft.TimeStamp.Ticks).Y(ft => ft.Green);
            lsBlue.Configuration = Mappers.Xy<FactoryTelemetry>().X(ft => ft.TimeStamp.Ticks).Y(ft => ft.Blue);

            DataContext = this;
        }

        public ChartValues<FactoryTelemetry> ChartValues { get; set; } = new ChartValues<FactoryTelemetry>();

        private bool readingData = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!readingData)
            {
                Task.Factory.StartNew(ReadData);
            }
            readingData = !readingData;
        }

        private void ReadData()
        {
            // TODO: Populate the collection ChartValues

            string fileName = @"C:\Users\Millennium Singha\Downloads\Ex_Files_Code_Clinic_C_Sharp\Ex_Files_Code_Clinic_C_Sharp\Exercise Files\Ch06\dashBoardData.csv";

            foreach (var ft in FactoryTelemetry.Load(fileName))
            {
                if (!readingData)
                    return;

                ChartValues.Add(ft);

                this.EngineEfficiency = ft.Efficiency;

                AdjustAxis(ft.TimeStamp.Ticks);

                if (ChartValues.Count > 30)
                    ChartValues.RemoveAt(0);

                Thread.Sleep(50);
            }
        }

        public double AxisStep { get; set; } = TimeSpan.FromSeconds(5).Ticks;
        public double AxisUnit { get; set; } = TimeSpan.FromSeconds(1).Ticks;

        private double axisMax = tickZero + TimeSpan.FromSeconds(30).Ticks;
        public double AxisMax { get => axisMax; set { axisMax = value; OnPropertyChanged(nameof(AxisMax));  } }

        private double axisMin = tickZero;
        public double AxisMin { get => axisMin; set { axisMin = value; OnPropertyChanged(nameof(AxisMin)); } }

        private void AdjustAxis(long ticks)
        {
            var width = TimeSpan.FromSeconds(30).Ticks;

            AxisMin = (ticks - tickZero < width) ? tickZero : ticks - width;
            AxisMax = (ticks - tickZero < width) ? tickZero + width : ticks;
        }

        private double _EngineEfficiency = 65;

        public double EngineEfficiency
        {
            get
            {
                return _EngineEfficiency;
            }
            set
            {
                _EngineEfficiency = value;
                OnPropertyChanged(nameof(EngineEfficiency));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
