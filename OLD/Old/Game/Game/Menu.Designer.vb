﻿'------------------------------------------------------------------------------
' <auto-generated>
'     這段程式碼是由工具產生的。
'     執行階段版本:4.0.30319.42000
'
'     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
'     變更將會遺失。
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace My.Resources
    
    '這個類別是自動產生的，是利用 StronglyTypedResourceBuilder
    '類別透過 ResGen 或 Visual Studio 這類工具。
    '若要加入或移除成員，請編輯您的 .ResX 檔，然後重新執行 ResGen
    '(利用 /str 選項)，或重建您的 VS 專案。
    '''<summary>
    '''  用於查詢當地語系化字串等的強類型資源類別。
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Friend Class Menu
        
        Private Shared resourceMan As Global.System.Resources.ResourceManager
        
        Private Shared resourceCulture As Global.System.Globalization.CultureInfo
        
        <Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>  _
        Friend Sub New()
            MyBase.New
        End Sub
        
        '''<summary>
        '''  傳回這個類別使用的快取的 ResourceManager 執行個體。
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Shared ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("Game.Menu", GetType(Menu).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''  覆寫目前執行緒的 CurrentUICulture 屬性，對象是所有
        '''  使用這個強類型資源類別的資源查閱。
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Shared Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''  查詢類型 System.Drawing.Bitmap 的當地語系化資源。
        '''</summary>
        Friend Shared ReadOnly Property _Exit() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("_Exit", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  查詢類型 System.Drawing.Bitmap 的當地語系化資源。
        '''</summary>
        Friend Shared ReadOnly Property bg() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("bg", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  查詢類型 System.Drawing.Bitmap 的當地語系化資源。
        '''</summary>
        Friend Shared ReadOnly Property Copyright() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("Copyright", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  查詢類型 System.Drawing.Bitmap 的當地語系化資源。
        '''</summary>
        Friend Shared ReadOnly Property DoneSetting() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("DoneSetting", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  查詢類型 System.Drawing.Bitmap 的當地語系化資源。
        '''</summary>
        Friend Shared ReadOnly Property DoneSettingActivated() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("DoneSettingActivated", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  查詢類型 System.Drawing.Bitmap 的當地語系化資源。
        '''</summary>
        Friend Shared ReadOnly Property Enter() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("Enter", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  查詢類型 System.Drawing.Bitmap 的當地語系化資源。
        '''</summary>
        Friend Shared ReadOnly Property ExitActivated() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("ExitActivated", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  查詢類型 System.Drawing.Bitmap 的當地語系化資源。
        '''</summary>
        Friend Shared ReadOnly Property Logo() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("Logo", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  查詢類型 System.Drawing.Bitmap 的當地語系化資源。
        '''</summary>
        Friend Shared ReadOnly Property Setting() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("Setting", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  查詢類型 System.Drawing.Bitmap 的當地語系化資源。
        '''</summary>
        Friend Shared ReadOnly Property SettingActivated() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("SettingActivated", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  查詢類型 System.Drawing.Bitmap 的當地語系化資源。
        '''</summary>
        Friend Shared ReadOnly Property Version() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("Version", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
    End Class
End Namespace
