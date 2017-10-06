using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using GraphX.Controls;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
using Microsoft.Win32;
using RabbitMQClient;
using RabbitMQClient.Messages;
using SqlPlusDbSync.Configuration;
using SqlPlusDbSync.Data.Database;
using SqlPlusDbSync.Live.Models;
using SqlPlusDbSync.Shared;

namespace SqlPlusDbSync.Live
{
    public class MainViewModel : IDisposable
    {
        private RabbitMQClient.Client _client;
        private GXLogicCoreExample _logicCore;
        private AsnaDatabaseContext _context;
        private object _lockObject;
        private ObservableCollection<ActiveSessionModel> _sessions;
        private Dispatcher _dispatcher;


        public MainViewModel(Dispatcher dispatcher)
        {
            Config.Instance.Load();

            _lockObject = new object();
            //GraphAreaExample_Setup();

            _context = new AsnaDatabaseContext(Config.Instance.ConnectionString);

            _client = new Client(Config.Instance.MessageServer, Config.Instance.MessageServerPort, int.MaxValue);
            _client.OnMessage += this._client_OnMessage;
            _client.Connect();

            _client.Subscribe(CommonHelper.InfoChannelName);

            _sessions = new ObservableCollection<ActiveSessionModel>();
            _dispatcher = dispatcher;
        }

        #region TableMode

        public ObservableCollection<ActiveSessionModel> Sessions => _sessions;

        #endregion

        private void _client_OnMessage(object sender, MessageEventArgs args)
        {
            lock (_lockObject)
            {
                if (!_dispatcher.HasShutdownStarted)
                    _dispatcher.Invoke(() =>
                    {
                        if (_client.IsConnected)
                        {
                            if (args.Message is InfoConnectMessage)
                            {
                                var msg = args.Message as InfoConnectMessage;
                                _sessions.Add(new ActiveSessionModel { From = args.Message.From, Type = args.Message, EventTime = DateTime.Now, Size = msg.Size, Info = msg.Info });
                            }
                            else if (args.Message is InfoMessageReciveMessage)
                            {
                                var msg = args.Message as InfoMessageReciveMessage;
                                _sessions.Add(new ActiveSessionModel { From = msg.From, Type = msg, To = msg.To, EventTime = DateTime.Now, Size = msg.Size, Info = msg.Info });
                            }
                            else if (args.Message is InfoMessageProcessedMessage)
                            {
                                var msg = args.Message as InfoMessageProcessedMessage;
                                _sessions.Add(new ActiveSessionModel { From = msg.From, Type = args.Message, EventTime = DateTime.Now, Size = msg.Size, Info = msg.Info });
                            }
                        }
                    });
                //if (args.Message is InfoConnectMessage)
                //{
                //    var msg = args.Message as InfoConnectMessage;
                //    var element = _logicCore.Graph.Vertices.FirstOrDefault(x => x.Text == msg.From.ToString());
                //    if (element is null)
                //        _logicCore.Graph.AddVertex(new DataVertex(msg.From.ToString()) { Name = _context.GetNameFromGuid(msg.From) });
                //}
                //else if (args.Message is InfoMessageReciveMessage)
                //{
                //    var msg = args.Message as InfoMessageReciveMessage;

                //    var elementFrom = _logicCore.Graph.Vertices.FirstOrDefault(x => x.Text == msg.From.ToString());
                //    var elementTo = _logicCore.Graph.Vertices.FirstOrDefault(x => x.Text == msg.To.ToString());

                //    if (elementFrom is null)
                //    {
                //        elementFrom = new DataVertex(msg.From.ToString());
                //        elementFrom.Name = _context.GetNameFromGuid(msg.From);
                //        _logicCore.Graph.AddVertex(elementFrom);
                //    }

                //    if (elementTo is null)
                //    {
                //        elementTo = new DataVertex(msg.To.ToString());
                //        elementTo.Name = _context.GetNameFromGuid(msg.To);
                //        _logicCore.Graph.AddVertex(elementTo);
                //    }

                //    var edge = new DataEdge(elementFrom, elementTo) { IsLive = true, MessageId = msg.Id };
                //    _logicCore.Graph.AddEdge(edge);
                //}
                //else if (args.Message is InfoMessageProcessedMessage)
                //{
                //    var msg = args.Message as InfoMessageProcessedMessage;
                //    _logicCore.Graph.RemoveEdgeIf(x => x.MessageId == msg.Id);
                //}

                //OnGraphChanged?.Invoke(this, null);
                args.Handled = true;
            }
        }

        #region MapMode



        public GXLogicCoreExample LogicCore => _logicCore;

        private GraphExample GraphExample_Setup()
        {
            //Lets make new data graph instance
            var dataGraph = new GraphExample();
            //Now we need to create edges and vertices to fill data graph
            //This edges and vertices will represent graph structure and connections
            //Lets make some vertices

            //for (int i = 1; i < 10; i++)
            //{
            //    //Create new vertex with specified Text. Also we will assign custom unique ID.
            //    //This ID is needed for several features such as serialization and edge routing algorithms.
            //    //If you don't need any custom IDs and you are using automatic Area.GenerateGraph() method then you can skip ID assignment
            //    //because specified method automaticaly assigns missing data ids (this behavior is controlled by method param).
            //    var dataVertex = new DataVertex("MyVertex " + i);
            //    //Add vertex to data graph
            //    dataGraph.AddVertex(dataVertex);
            //}

            ////Now lets make some edges that will connect our vertices
            ////get the indexed list of graph vertices we have already added
            //var vlist = dataGraph.Vertices.ToList();
            ////Then create two edges optionaly defining Text property to show who are connected
            //var dataEdge = new DataEdge(vlist[0], vlist[1]) { PointFrom = string.Format("{0} -> {1}", vlist[0], vlist[1]), IsLive = true };
            //dataGraph.AddEdge(dataEdge);
            //dataEdge = new DataEdge(vlist[2], vlist[3]) { PointFrom = string.Format("{0} -> {1}", vlist[2], vlist[3]), IsLive = true };
            //dataGraph.AddEdge(dataEdge);

            //dataEdge = new DataEdge(vlist[3], vlist[2]) { PointFrom = string.Format("{0} -> {1}", vlist[2], vlist[3]), IsLive = true };
            //dataGraph.AddEdge(dataEdge);

            //dataEdge = new DataEdge(vlist[1], vlist[0]) { PointFrom = string.Format("{0} -> {1}", vlist[2], vlist[3]) };
            //dataGraph.AddEdge(dataEdge);

            //dataGraph.RemoveEdgeIf(x => x.Source == vlist[1]);

            return dataGraph;
        }

        private void GraphAreaExample_Setup()
        {
            //Lets create logic core and filled data graph with edges and vertices
            _logicCore = new GXLogicCoreExample() { Graph = GraphExample_Setup() };
            //This property sets layout algorithm that will be used to calculate vertices positions
            //Different algorithms uses different values and some of them uses edge Weight property.
            _logicCore.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.Circular;
            //Now we can set parameters for selected algorithm using AlgorithmFactory property. This property provides methods for
            //creating all available algorithms and algo parameters.
            //_logicCore.DefaultLayoutAlgorithmParams = _logicCore.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.KK);
            ////Unfortunately to change algo parameters you need to specify params type which is different for every algorithm.
            //((KKLayoutParameters)_logicCore.DefaultLayoutAlgorithmParams).MaxIterations = 100;



            //This property sets vertex overlap removal algorithm.
            //Such algorithms help to arrange vertices in the layout so no one overlaps each other.
            _logicCore.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;

            //Default parameters are created automaticaly when new default algorithm is set and previous params were NULL
            _logicCore.DefaultOverlapRemovalAlgorithmParams.HorizontalGap = 50;
            _logicCore.DefaultOverlapRemovalAlgorithmParams.VerticalGap = 50;

            //This property sets edge routing algorithm that is used to build route paths according to algorithm logic.
            //For ex., SimpleER algorithm will try to set edge paths around vertices so no edge will intersect any vertex.
            //Bundling algorithm will try to tie different edges that follows same direction to a single channel making complex graphs more appealing.
            _logicCore.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.None;
            _logicCore.EdgeCurvingEnabled = true;
            _logicCore.EnableParallelEdges = true;
            _logicCore.ParallelEdgeDistance = 10;
            //This property sets async algorithms computation so methods like: Area.RelayoutGraph() and Area.GenerateGraph()
            //will run async with the UI thread. Completion of the specified methods can be catched by corresponding events:
            //Area.RelayoutFinished and Area.GenerateGraphFinished.
            _logicCore.AsyncAlgorithmCompute = true;

        }

        public event EventHandler<object, EventArgs> OnGraphChanged;

        #endregion

        public void Dispose()
        {
            _client.OnMessage -= this._client_OnMessage;
            _client.Dispose();
        }
    }
}