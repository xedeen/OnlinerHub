﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.33440
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.Silverlight.Phone.ServiceReference, version 3.7.0.0
// 
namespace Onliner.OnlinerHub {
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CommentsPageDto", Namespace="http://schemas.datacontract.org/2004/07/CacheHub")]
    public partial class CommentsPageDto : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string ErrorField;
        
        private System.Collections.ObjectModel.ObservableCollection<Onliner.OnlinerHub.CommentDto> commentsField;
        
        private System.Nullable<int> next_page_cursorField;
        
        private System.Nullable<int> previous_page_cursorField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Error {
            get {
                return this.ErrorField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorField, value) != true)) {
                    this.ErrorField = value;
                    this.RaisePropertyChanged("Error");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.ObjectModel.ObservableCollection<Onliner.OnlinerHub.CommentDto> comments {
            get {
                return this.commentsField;
            }
            set {
                if ((object.ReferenceEquals(this.commentsField, value) != true)) {
                    this.commentsField = value;
                    this.RaisePropertyChanged("comments");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<int> next_page_cursor {
            get {
                return this.next_page_cursorField;
            }
            set {
                if ((this.next_page_cursorField.Equals(value) != true)) {
                    this.next_page_cursorField = value;
                    this.RaisePropertyChanged("next_page_cursor");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<int> previous_page_cursor {
            get {
                return this.previous_page_cursorField;
            }
            set {
                if ((this.previous_page_cursorField.Equals(value) != true)) {
                    this.previous_page_cursorField = value;
                    this.RaisePropertyChanged("previous_page_cursor");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CommentDto", Namespace="http://schemas.datacontract.org/2004/07/CacheHub")]
    public partial class CommentDto : object, System.ComponentModel.INotifyPropertyChanged {
        
        private Onliner.OnlinerHub.AuthorDto authorField;
        
        private string contentField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Onliner.OnlinerHub.AuthorDto author {
            get {
                return this.authorField;
            }
            set {
                if ((object.ReferenceEquals(this.authorField, value) != true)) {
                    this.authorField = value;
                    this.RaisePropertyChanged("author");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string content {
            get {
                return this.contentField;
            }
            set {
                if ((object.ReferenceEquals(this.contentField, value) != true)) {
                    this.contentField = value;
                    this.RaisePropertyChanged("content");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="AuthorDto", Namespace="http://schemas.datacontract.org/2004/07/CacheHub")]
    public partial class AuthorDto : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string avatar_source_uriField;
        
        private string nameField;
        
        private string profile_uriField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string avatar_source_uri {
            get {
                return this.avatar_source_uriField;
            }
            set {
                if ((object.ReferenceEquals(this.avatar_source_uriField, value) != true)) {
                    this.avatar_source_uriField = value;
                    this.RaisePropertyChanged("avatar_source_uri");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string name {
            get {
                return this.nameField;
            }
            set {
                if ((object.ReferenceEquals(this.nameField, value) != true)) {
                    this.nameField = value;
                    this.RaisePropertyChanged("name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string profile_uri {
            get {
                return this.profile_uriField;
            }
            set {
                if ((object.ReferenceEquals(this.profile_uriField, value) != true)) {
                    this.profile_uriField = value;
                    this.RaisePropertyChanged("profile_uri");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="OnlinerHub.IOnlinerHub")]
    public interface IOnlinerHub {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IOnlinerHub/GetReadability", ReplyAction="http://tempuri.org/IOnlinerHub/GetReadabilityResponse")]
        System.IAsyncResult BeginGetReadability(string articleUrl, System.AsyncCallback callback, object asyncState);
        
        string EndGetReadability(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IOnlinerHub/GetComments", ReplyAction="http://tempuri.org/IOnlinerHub/GetCommentsResponse")]
        System.IAsyncResult BeginGetComments(string articleUrl, int cursor, System.AsyncCallback callback, object asyncState);
        
        Onliner.OnlinerHub.CommentsPageDto EndGetComments(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IOnlinerHubChannel : Onliner.OnlinerHub.IOnlinerHub, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetReadabilityCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetReadabilityCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public string Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetCommentsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetCommentsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public Onliner.OnlinerHub.CommentsPageDto Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((Onliner.OnlinerHub.CommentsPageDto)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class OnlinerHubClient : System.ServiceModel.ClientBase<Onliner.OnlinerHub.IOnlinerHub>, Onliner.OnlinerHub.IOnlinerHub {
        
        private BeginOperationDelegate onBeginGetReadabilityDelegate;
        
        private EndOperationDelegate onEndGetReadabilityDelegate;
        
        private System.Threading.SendOrPostCallback onGetReadabilityCompletedDelegate;
        
        private BeginOperationDelegate onBeginGetCommentsDelegate;
        
        private EndOperationDelegate onEndGetCommentsDelegate;
        
        private System.Threading.SendOrPostCallback onGetCommentsCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public OnlinerHubClient() {
        }
        
        public OnlinerHubClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public OnlinerHubClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OnlinerHubClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OnlinerHubClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Net.CookieContainer CookieContainer {
            get {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    return httpCookieContainerManager.CookieContainer;
                }
                else {
                    return null;
                }
            }
            set {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    httpCookieContainerManager.CookieContainer = value;
                }
                else {
                    throw new System.InvalidOperationException("Unable to set the CookieContainer. Please make sure the binding contains an HttpC" +
                            "ookieContainerBindingElement.");
                }
            }
        }
        
        public event System.EventHandler<GetReadabilityCompletedEventArgs> GetReadabilityCompleted;
        
        public event System.EventHandler<GetCommentsCompletedEventArgs> GetCommentsCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult Onliner.OnlinerHub.IOnlinerHub.BeginGetReadability(string articleUrl, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetReadability(articleUrl, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        string Onliner.OnlinerHub.IOnlinerHub.EndGetReadability(System.IAsyncResult result) {
            return base.Channel.EndGetReadability(result);
        }
        
        private System.IAsyncResult OnBeginGetReadability(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string articleUrl = ((string)(inValues[0]));
            return ((Onliner.OnlinerHub.IOnlinerHub)(this)).BeginGetReadability(articleUrl, callback, asyncState);
        }
        
        private object[] OnEndGetReadability(System.IAsyncResult result) {
            string retVal = ((Onliner.OnlinerHub.IOnlinerHub)(this)).EndGetReadability(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetReadabilityCompleted(object state) {
            if ((this.GetReadabilityCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetReadabilityCompleted(this, new GetReadabilityCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetReadabilityAsync(string articleUrl) {
            this.GetReadabilityAsync(articleUrl, null);
        }
        
        public void GetReadabilityAsync(string articleUrl, object userState) {
            if ((this.onBeginGetReadabilityDelegate == null)) {
                this.onBeginGetReadabilityDelegate = new BeginOperationDelegate(this.OnBeginGetReadability);
            }
            if ((this.onEndGetReadabilityDelegate == null)) {
                this.onEndGetReadabilityDelegate = new EndOperationDelegate(this.OnEndGetReadability);
            }
            if ((this.onGetReadabilityCompletedDelegate == null)) {
                this.onGetReadabilityCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetReadabilityCompleted);
            }
            base.InvokeAsync(this.onBeginGetReadabilityDelegate, new object[] {
                        articleUrl}, this.onEndGetReadabilityDelegate, this.onGetReadabilityCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult Onliner.OnlinerHub.IOnlinerHub.BeginGetComments(string articleUrl, int cursor, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetComments(articleUrl, cursor, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Onliner.OnlinerHub.CommentsPageDto Onliner.OnlinerHub.IOnlinerHub.EndGetComments(System.IAsyncResult result) {
            return base.Channel.EndGetComments(result);
        }
        
        private System.IAsyncResult OnBeginGetComments(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string articleUrl = ((string)(inValues[0]));
            int cursor = ((int)(inValues[1]));
            return ((Onliner.OnlinerHub.IOnlinerHub)(this)).BeginGetComments(articleUrl, cursor, callback, asyncState);
        }
        
        private object[] OnEndGetComments(System.IAsyncResult result) {
            Onliner.OnlinerHub.CommentsPageDto retVal = ((Onliner.OnlinerHub.IOnlinerHub)(this)).EndGetComments(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetCommentsCompleted(object state) {
            if ((this.GetCommentsCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetCommentsCompleted(this, new GetCommentsCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetCommentsAsync(string articleUrl, int cursor) {
            this.GetCommentsAsync(articleUrl, cursor, null);
        }
        
        public void GetCommentsAsync(string articleUrl, int cursor, object userState) {
            if ((this.onBeginGetCommentsDelegate == null)) {
                this.onBeginGetCommentsDelegate = new BeginOperationDelegate(this.OnBeginGetComments);
            }
            if ((this.onEndGetCommentsDelegate == null)) {
                this.onEndGetCommentsDelegate = new EndOperationDelegate(this.OnEndGetComments);
            }
            if ((this.onGetCommentsCompletedDelegate == null)) {
                this.onGetCommentsCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetCommentsCompleted);
            }
            base.InvokeAsync(this.onBeginGetCommentsDelegate, new object[] {
                        articleUrl,
                        cursor}, this.onEndGetCommentsDelegate, this.onGetCommentsCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
        }
        
        private object[] OnEndOpen(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
            return null;
        }
        
        private void OnOpenCompleted(object state) {
            if ((this.OpenCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void OpenAsync() {
            this.OpenAsync(null);
        }
        
        public void OpenAsync(object userState) {
            if ((this.onBeginOpenDelegate == null)) {
                this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
            }
            if ((this.onEndOpenDelegate == null)) {
                this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
            }
            if ((this.onOpenCompletedDelegate == null)) {
                this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
            }
            base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
        }
        
        private object[] OnEndClose(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
            return null;
        }
        
        private void OnCloseCompleted(object state) {
            if ((this.CloseCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void CloseAsync() {
            this.CloseAsync(null);
        }
        
        public void CloseAsync(object userState) {
            if ((this.onBeginCloseDelegate == null)) {
                this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
            }
            if ((this.onEndCloseDelegate == null)) {
                this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
            }
            if ((this.onCloseCompletedDelegate == null)) {
                this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
            }
            base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
        }
        
        protected override Onliner.OnlinerHub.IOnlinerHub CreateChannel() {
            return new OnlinerHubClientChannel(this);
        }
        
        private class OnlinerHubClientChannel : ChannelBase<Onliner.OnlinerHub.IOnlinerHub>, Onliner.OnlinerHub.IOnlinerHub {
            
            public OnlinerHubClientChannel(System.ServiceModel.ClientBase<Onliner.OnlinerHub.IOnlinerHub> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginGetReadability(string articleUrl, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = articleUrl;
                System.IAsyncResult _result = base.BeginInvoke("GetReadability", _args, callback, asyncState);
                return _result;
            }
            
            public string EndGetReadability(System.IAsyncResult result) {
                object[] _args = new object[0];
                string _result = ((string)(base.EndInvoke("GetReadability", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginGetComments(string articleUrl, int cursor, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[2];
                _args[0] = articleUrl;
                _args[1] = cursor;
                System.IAsyncResult _result = base.BeginInvoke("GetComments", _args, callback, asyncState);
                return _result;
            }
            
            public Onliner.OnlinerHub.CommentsPageDto EndGetComments(System.IAsyncResult result) {
                object[] _args = new object[0];
                Onliner.OnlinerHub.CommentsPageDto _result = ((Onliner.OnlinerHub.CommentsPageDto)(base.EndInvoke("GetComments", _args, result)));
                return _result;
            }
        }
    }
}