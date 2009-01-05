using System.ServiceModel;
using System.Web.Script.Services;
using AdamRoderick.HeatMap;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System;

namespace CenStatsHeatMap.UI
{
    [ServiceContract(Namespace = "HeatMap.Services")]
    public interface IHeatMapService
    {

        [OperationContract]
        [WebInvoke(
            Method = "POST"
            , BodyStyle = WebMessageBodyStyle.Wrapped
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            )]
        Guid GenerateHeatMapOverlayToken(HeatPoint[] heatPoints, HeatmapSettings settings);

        [OperationContract]
        [WebInvoke(
            Method = "POST"
            , BodyStyle = WebMessageBodyStyle.Wrapped
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            )]
        Guid UpdateHeatmapOverlaySettings(Guid token, HeatmapSettings settings);

        [OperationContract]
        [WebInvoke(
            Method = "POST"
            , BodyStyle = WebMessageBodyStyle.Wrapped
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            )]
        Guid UpdateHeatmapOverlayHeatPoints(Guid token, HeatPoint[] heatPoints);

        [OperationContract]
        [WebInvoke(
            Method = "POST"
            , BodyStyle = WebMessageBodyStyle.Wrapped
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            )]
        HeatmapSettings RetrieveHeatMapOverlaySettings(Guid token);

        [OperationContract]
        [WebInvoke(
            Method="POST"
            , BodyStyle = WebMessageBodyStyle.Bare
            , ResponseFormat = WebMessageFormat.Json
            , RequestFormat = WebMessageFormat.Json
            )]
        void DoWork(Person p);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        Person GetPerson();
    }

}