﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nomina.Procesador.webServicePAC {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ArrayOfString", Namespace="http://facturadorelectronico.com/timbrado/", ItemName="string")]
    [System.SerializableAttribute()]
    public class ArrayOfString : System.Collections.Generic.List<string> {
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://facturadorelectronico.com/timbrado/", ConfigurationName="webServicePAC.wsTimbradoSoap")]
    public interface wsTimbradoSoap {
        
        // CODEGEN: Se está generando un contrato de mensaje, ya que el nombre de elemento xmlCancelacion del espacio de nombres http://facturadorelectronico.com/timbrado/ no está marcado para aceptar valores nil.
        [System.ServiceModel.OperationContractAttribute(Action="http://facturadorelectronico.com/timbrado/cancelarComprobante", ReplyAction="*")]
        Nomina.Procesador.webServicePAC.cancelarComprobanteResponse cancelarComprobante(Nomina.Procesador.webServicePAC.cancelarComprobanteRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://facturadorelectronico.com/timbrado/cancelarComprobante", ReplyAction="*")]
        System.Threading.Tasks.Task<Nomina.Procesador.webServicePAC.cancelarComprobanteResponse> cancelarComprobanteAsync(Nomina.Procesador.webServicePAC.cancelarComprobanteRequest request);
        
        // CODEGEN: Se está generando un contrato de mensaje, ya que el nombre de elemento xmlCancelacion del espacio de nombres http://facturadorelectronico.com/timbrado/ no está marcado para aceptar valores nil.
        [System.ServiceModel.OperationContractAttribute(Action="http://facturadorelectronico.com/timbrado/CancelarComprobanteRetenciones", ReplyAction="*")]
        Nomina.Procesador.webServicePAC.CancelarComprobanteRetencionesResponse CancelarComprobanteRetenciones(Nomina.Procesador.webServicePAC.CancelarComprobanteRetencionesRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://facturadorelectronico.com/timbrado/CancelarComprobanteRetenciones", ReplyAction="*")]
        System.Threading.Tasks.Task<Nomina.Procesador.webServicePAC.CancelarComprobanteRetencionesResponse> CancelarComprobanteRetencionesAsync(Nomina.Procesador.webServicePAC.CancelarComprobanteRetencionesRequest request);
        
        // CODEGEN: Se está generando un contrato de mensaje, ya que el nombre de elemento usuario del espacio de nombres http://facturadorelectronico.com/timbrado/ no está marcado para aceptar valores nil.
        [System.ServiceModel.OperationContractAttribute(Action="http://facturadorelectronico.com/timbrado/EnviarCancelacionPFX", ReplyAction="*")]
        Nomina.Procesador.webServicePAC.EnviarCancelacionPFXResponse EnviarCancelacionPFX(Nomina.Procesador.webServicePAC.EnviarCancelacionPFXRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://facturadorelectronico.com/timbrado/EnviarCancelacionPFX", ReplyAction="*")]
        System.Threading.Tasks.Task<Nomina.Procesador.webServicePAC.EnviarCancelacionPFXResponse> EnviarCancelacionPFXAsync(Nomina.Procesador.webServicePAC.EnviarCancelacionPFXRequest request);
        
        // CODEGEN: Se está generando un contrato de mensaje, ya que el nombre de elemento CFDIcliente del espacio de nombres http://facturadorelectronico.com/timbrado/ no está marcado para aceptar valores nil.
        [System.ServiceModel.OperationContractAttribute(Action="http://facturadorelectronico.com/timbrado/obtenerTimbrado", ReplyAction="*")]
        Nomina.Procesador.webServicePAC.obtenerTimbradoResponse obtenerTimbrado(Nomina.Procesador.webServicePAC.obtenerTimbradoRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://facturadorelectronico.com/timbrado/obtenerTimbrado", ReplyAction="*")]
        System.Threading.Tasks.Task<Nomina.Procesador.webServicePAC.obtenerTimbradoResponse> obtenerTimbradoAsync(Nomina.Procesador.webServicePAC.obtenerTimbradoRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class cancelarComprobanteRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="cancelarComprobante", Namespace="http://facturadorelectronico.com/timbrado/", Order=0)]
        public Nomina.Procesador.webServicePAC.cancelarComprobanteRequestBody Body;
        
        public cancelarComprobanteRequest() {
        }
        
        public cancelarComprobanteRequest(Nomina.Procesador.webServicePAC.cancelarComprobanteRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://facturadorelectronico.com/timbrado/")]
    public partial class cancelarComprobanteRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string xmlCancelacion;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string usuario;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string password;
        
        public cancelarComprobanteRequestBody() {
        }
        
        public cancelarComprobanteRequestBody(string xmlCancelacion, string usuario, string password) {
            this.xmlCancelacion = xmlCancelacion;
            this.usuario = usuario;
            this.password = password;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class cancelarComprobanteResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="cancelarComprobanteResponse", Namespace="http://facturadorelectronico.com/timbrado/", Order=0)]
        public Nomina.Procesador.webServicePAC.cancelarComprobanteResponseBody Body;
        
        public cancelarComprobanteResponse() {
        }
        
        public cancelarComprobanteResponse(Nomina.Procesador.webServicePAC.cancelarComprobanteResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://facturadorelectronico.com/timbrado/")]
    public partial class cancelarComprobanteResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public System.Xml.Linq.XElement cancelarComprobanteResult;
        
        public cancelarComprobanteResponseBody() {
        }
        
        public cancelarComprobanteResponseBody(System.Xml.Linq.XElement cancelarComprobanteResult) {
            this.cancelarComprobanteResult = cancelarComprobanteResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class CancelarComprobanteRetencionesRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="CancelarComprobanteRetenciones", Namespace="http://facturadorelectronico.com/timbrado/", Order=0)]
        public Nomina.Procesador.webServicePAC.CancelarComprobanteRetencionesRequestBody Body;
        
        public CancelarComprobanteRetencionesRequest() {
        }
        
        public CancelarComprobanteRetencionesRequest(Nomina.Procesador.webServicePAC.CancelarComprobanteRetencionesRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://facturadorelectronico.com/timbrado/")]
    public partial class CancelarComprobanteRetencionesRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string xmlCancelacion;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string usuariotimbrado;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string passwordtimbrado;
        
        public CancelarComprobanteRetencionesRequestBody() {
        }
        
        public CancelarComprobanteRetencionesRequestBody(string xmlCancelacion, string usuariotimbrado, string passwordtimbrado) {
            this.xmlCancelacion = xmlCancelacion;
            this.usuariotimbrado = usuariotimbrado;
            this.passwordtimbrado = passwordtimbrado;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class CancelarComprobanteRetencionesResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="CancelarComprobanteRetencionesResponse", Namespace="http://facturadorelectronico.com/timbrado/", Order=0)]
        public Nomina.Procesador.webServicePAC.CancelarComprobanteRetencionesResponseBody Body;
        
        public CancelarComprobanteRetencionesResponse() {
        }
        
        public CancelarComprobanteRetencionesResponse(Nomina.Procesador.webServicePAC.CancelarComprobanteRetencionesResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://facturadorelectronico.com/timbrado/")]
    public partial class CancelarComprobanteRetencionesResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public System.Xml.Linq.XElement CancelarComprobanteRetencionesResult;
        
        public CancelarComprobanteRetencionesResponseBody() {
        }
        
        public CancelarComprobanteRetencionesResponseBody(System.Xml.Linq.XElement CancelarComprobanteRetencionesResult) {
            this.CancelarComprobanteRetencionesResult = CancelarComprobanteRetencionesResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class EnviarCancelacionPFXRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="EnviarCancelacionPFX", Namespace="http://facturadorelectronico.com/timbrado/", Order=0)]
        public Nomina.Procesador.webServicePAC.EnviarCancelacionPFXRequestBody Body;
        
        public EnviarCancelacionPFXRequest() {
        }
        
        public EnviarCancelacionPFXRequest(Nomina.Procesador.webServicePAC.EnviarCancelacionPFXRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://facturadorelectronico.com/timbrado/")]
    public partial class EnviarCancelacionPFXRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string usuario;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string password;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public Nomina.Procesador.webServicePAC.ArrayOfString uuids;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public byte[] pfx;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string passwordPfx;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=5)]
        public string rfc;
        
        public EnviarCancelacionPFXRequestBody() {
        }
        
        public EnviarCancelacionPFXRequestBody(string usuario, string password, Nomina.Procesador.webServicePAC.ArrayOfString uuids, byte[] pfx, string passwordPfx, string rfc) {
            this.usuario = usuario;
            this.password = password;
            this.uuids = uuids;
            this.pfx = pfx;
            this.passwordPfx = passwordPfx;
            this.rfc = rfc;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class EnviarCancelacionPFXResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="EnviarCancelacionPFXResponse", Namespace="http://facturadorelectronico.com/timbrado/", Order=0)]
        public Nomina.Procesador.webServicePAC.EnviarCancelacionPFXResponseBody Body;
        
        public EnviarCancelacionPFXResponse() {
        }
        
        public EnviarCancelacionPFXResponse(Nomina.Procesador.webServicePAC.EnviarCancelacionPFXResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://facturadorelectronico.com/timbrado/")]
    public partial class EnviarCancelacionPFXResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public System.Xml.Linq.XElement EnviarCancelacionPFXResult;
        
        public EnviarCancelacionPFXResponseBody() {
        }
        
        public EnviarCancelacionPFXResponseBody(System.Xml.Linq.XElement EnviarCancelacionPFXResult) {
            this.EnviarCancelacionPFXResult = EnviarCancelacionPFXResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class obtenerTimbradoRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="obtenerTimbrado", Namespace="http://facturadorelectronico.com/timbrado/", Order=0)]
        public Nomina.Procesador.webServicePAC.obtenerTimbradoRequestBody Body;
        
        public obtenerTimbradoRequest() {
        }
        
        public obtenerTimbradoRequest(Nomina.Procesador.webServicePAC.obtenerTimbradoRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://facturadorelectronico.com/timbrado/")]
    public partial class obtenerTimbradoRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string CFDIcliente;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string Usuario;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string password;
        
        public obtenerTimbradoRequestBody() {
        }
        
        public obtenerTimbradoRequestBody(string CFDIcliente, string Usuario, string password) {
            this.CFDIcliente = CFDIcliente;
            this.Usuario = Usuario;
            this.password = password;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class obtenerTimbradoResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="obtenerTimbradoResponse", Namespace="http://facturadorelectronico.com/timbrado/", Order=0)]
        public Nomina.Procesador.webServicePAC.obtenerTimbradoResponseBody Body;
        
        public obtenerTimbradoResponse() {
        }
        
        public obtenerTimbradoResponse(Nomina.Procesador.webServicePAC.obtenerTimbradoResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://facturadorelectronico.com/timbrado/")]
    public partial class obtenerTimbradoResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public System.Xml.Linq.XElement obtenerTimbradoResult;
        
        public obtenerTimbradoResponseBody() {
        }
        
        public obtenerTimbradoResponseBody(System.Xml.Linq.XElement obtenerTimbradoResult) {
            this.obtenerTimbradoResult = obtenerTimbradoResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface wsTimbradoSoapChannel : Nomina.Procesador.webServicePAC.wsTimbradoSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class wsTimbradoSoapClient : System.ServiceModel.ClientBase<Nomina.Procesador.webServicePAC.wsTimbradoSoap>, Nomina.Procesador.webServicePAC.wsTimbradoSoap {
        
        public wsTimbradoSoapClient() {
        }
        
        public wsTimbradoSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public wsTimbradoSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public wsTimbradoSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public wsTimbradoSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Nomina.Procesador.webServicePAC.cancelarComprobanteResponse Nomina.Procesador.webServicePAC.wsTimbradoSoap.cancelarComprobante(Nomina.Procesador.webServicePAC.cancelarComprobanteRequest request) {
            return base.Channel.cancelarComprobante(request);
        }
        
        public System.Xml.Linq.XElement cancelarComprobante(string xmlCancelacion, string usuario, string password) {
            Nomina.Procesador.webServicePAC.cancelarComprobanteRequest inValue = new Nomina.Procesador.webServicePAC.cancelarComprobanteRequest();
            inValue.Body = new Nomina.Procesador.webServicePAC.cancelarComprobanteRequestBody();
            inValue.Body.xmlCancelacion = xmlCancelacion;
            inValue.Body.usuario = usuario;
            inValue.Body.password = password;
            Nomina.Procesador.webServicePAC.cancelarComprobanteResponse retVal = ((Nomina.Procesador.webServicePAC.wsTimbradoSoap)(this)).cancelarComprobante(inValue);
            return retVal.Body.cancelarComprobanteResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Nomina.Procesador.webServicePAC.cancelarComprobanteResponse> Nomina.Procesador.webServicePAC.wsTimbradoSoap.cancelarComprobanteAsync(Nomina.Procesador.webServicePAC.cancelarComprobanteRequest request) {
            return base.Channel.cancelarComprobanteAsync(request);
        }
        
        public System.Threading.Tasks.Task<Nomina.Procesador.webServicePAC.cancelarComprobanteResponse> cancelarComprobanteAsync(string xmlCancelacion, string usuario, string password) {
            Nomina.Procesador.webServicePAC.cancelarComprobanteRequest inValue = new Nomina.Procesador.webServicePAC.cancelarComprobanteRequest();
            inValue.Body = new Nomina.Procesador.webServicePAC.cancelarComprobanteRequestBody();
            inValue.Body.xmlCancelacion = xmlCancelacion;
            inValue.Body.usuario = usuario;
            inValue.Body.password = password;
            return ((Nomina.Procesador.webServicePAC.wsTimbradoSoap)(this)).cancelarComprobanteAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Nomina.Procesador.webServicePAC.CancelarComprobanteRetencionesResponse Nomina.Procesador.webServicePAC.wsTimbradoSoap.CancelarComprobanteRetenciones(Nomina.Procesador.webServicePAC.CancelarComprobanteRetencionesRequest request) {
            return base.Channel.CancelarComprobanteRetenciones(request);
        }
        
        public System.Xml.Linq.XElement CancelarComprobanteRetenciones(string xmlCancelacion, string usuariotimbrado, string passwordtimbrado) {
            Nomina.Procesador.webServicePAC.CancelarComprobanteRetencionesRequest inValue = new Nomina.Procesador.webServicePAC.CancelarComprobanteRetencionesRequest();
            inValue.Body = new Nomina.Procesador.webServicePAC.CancelarComprobanteRetencionesRequestBody();
            inValue.Body.xmlCancelacion = xmlCancelacion;
            inValue.Body.usuariotimbrado = usuariotimbrado;
            inValue.Body.passwordtimbrado = passwordtimbrado;
            Nomina.Procesador.webServicePAC.CancelarComprobanteRetencionesResponse retVal = ((Nomina.Procesador.webServicePAC.wsTimbradoSoap)(this)).CancelarComprobanteRetenciones(inValue);
            return retVal.Body.CancelarComprobanteRetencionesResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Nomina.Procesador.webServicePAC.CancelarComprobanteRetencionesResponse> Nomina.Procesador.webServicePAC.wsTimbradoSoap.CancelarComprobanteRetencionesAsync(Nomina.Procesador.webServicePAC.CancelarComprobanteRetencionesRequest request) {
            return base.Channel.CancelarComprobanteRetencionesAsync(request);
        }
        
        public System.Threading.Tasks.Task<Nomina.Procesador.webServicePAC.CancelarComprobanteRetencionesResponse> CancelarComprobanteRetencionesAsync(string xmlCancelacion, string usuariotimbrado, string passwordtimbrado) {
            Nomina.Procesador.webServicePAC.CancelarComprobanteRetencionesRequest inValue = new Nomina.Procesador.webServicePAC.CancelarComprobanteRetencionesRequest();
            inValue.Body = new Nomina.Procesador.webServicePAC.CancelarComprobanteRetencionesRequestBody();
            inValue.Body.xmlCancelacion = xmlCancelacion;
            inValue.Body.usuariotimbrado = usuariotimbrado;
            inValue.Body.passwordtimbrado = passwordtimbrado;
            return ((Nomina.Procesador.webServicePAC.wsTimbradoSoap)(this)).CancelarComprobanteRetencionesAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Nomina.Procesador.webServicePAC.EnviarCancelacionPFXResponse Nomina.Procesador.webServicePAC.wsTimbradoSoap.EnviarCancelacionPFX(Nomina.Procesador.webServicePAC.EnviarCancelacionPFXRequest request) {
            return base.Channel.EnviarCancelacionPFX(request);
        }
        
        public System.Xml.Linq.XElement EnviarCancelacionPFX(string usuario, string password, Nomina.Procesador.webServicePAC.ArrayOfString uuids, byte[] pfx, string passwordPfx, string rfc) {
            Nomina.Procesador.webServicePAC.EnviarCancelacionPFXRequest inValue = new Nomina.Procesador.webServicePAC.EnviarCancelacionPFXRequest();
            inValue.Body = new Nomina.Procesador.webServicePAC.EnviarCancelacionPFXRequestBody();
            inValue.Body.usuario = usuario;
            inValue.Body.password = password;
            inValue.Body.uuids = uuids;
            inValue.Body.pfx = pfx;
            inValue.Body.passwordPfx = passwordPfx;
            inValue.Body.rfc = rfc;
            Nomina.Procesador.webServicePAC.EnviarCancelacionPFXResponse retVal = ((Nomina.Procesador.webServicePAC.wsTimbradoSoap)(this)).EnviarCancelacionPFX(inValue);
            return retVal.Body.EnviarCancelacionPFXResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Nomina.Procesador.webServicePAC.EnviarCancelacionPFXResponse> Nomina.Procesador.webServicePAC.wsTimbradoSoap.EnviarCancelacionPFXAsync(Nomina.Procesador.webServicePAC.EnviarCancelacionPFXRequest request) {
            return base.Channel.EnviarCancelacionPFXAsync(request);
        }
        
        public System.Threading.Tasks.Task<Nomina.Procesador.webServicePAC.EnviarCancelacionPFXResponse> EnviarCancelacionPFXAsync(string usuario, string password, Nomina.Procesador.webServicePAC.ArrayOfString uuids, byte[] pfx, string passwordPfx, string rfc) {
            Nomina.Procesador.webServicePAC.EnviarCancelacionPFXRequest inValue = new Nomina.Procesador.webServicePAC.EnviarCancelacionPFXRequest();
            inValue.Body = new Nomina.Procesador.webServicePAC.EnviarCancelacionPFXRequestBody();
            inValue.Body.usuario = usuario;
            inValue.Body.password = password;
            inValue.Body.uuids = uuids;
            inValue.Body.pfx = pfx;
            inValue.Body.passwordPfx = passwordPfx;
            inValue.Body.rfc = rfc;
            return ((Nomina.Procesador.webServicePAC.wsTimbradoSoap)(this)).EnviarCancelacionPFXAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Nomina.Procesador.webServicePAC.obtenerTimbradoResponse Nomina.Procesador.webServicePAC.wsTimbradoSoap.obtenerTimbrado(Nomina.Procesador.webServicePAC.obtenerTimbradoRequest request) {
            return base.Channel.obtenerTimbrado(request);
        }
        
        public System.Xml.Linq.XElement obtenerTimbrado(string CFDIcliente, string Usuario, string password) {
            Nomina.Procesador.webServicePAC.obtenerTimbradoRequest inValue = new Nomina.Procesador.webServicePAC.obtenerTimbradoRequest();
            inValue.Body = new Nomina.Procesador.webServicePAC.obtenerTimbradoRequestBody();
            inValue.Body.CFDIcliente = CFDIcliente;
            inValue.Body.Usuario = Usuario;
            inValue.Body.password = password;
            Nomina.Procesador.webServicePAC.obtenerTimbradoResponse retVal = ((Nomina.Procesador.webServicePAC.wsTimbradoSoap)(this)).obtenerTimbrado(inValue);
            return retVal.Body.obtenerTimbradoResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Nomina.Procesador.webServicePAC.obtenerTimbradoResponse> Nomina.Procesador.webServicePAC.wsTimbradoSoap.obtenerTimbradoAsync(Nomina.Procesador.webServicePAC.obtenerTimbradoRequest request) {
            return base.Channel.obtenerTimbradoAsync(request);
        }
        
        public System.Threading.Tasks.Task<Nomina.Procesador.webServicePAC.obtenerTimbradoResponse> obtenerTimbradoAsync(string CFDIcliente, string Usuario, string password) {
            Nomina.Procesador.webServicePAC.obtenerTimbradoRequest inValue = new Nomina.Procesador.webServicePAC.obtenerTimbradoRequest();
            inValue.Body = new Nomina.Procesador.webServicePAC.obtenerTimbradoRequestBody();
            inValue.Body.CFDIcliente = CFDIcliente;
            inValue.Body.Usuario = Usuario;
            inValue.Body.password = password;
            return ((Nomina.Procesador.webServicePAC.wsTimbradoSoap)(this)).obtenerTimbradoAsync(inValue);
        }
    }
}