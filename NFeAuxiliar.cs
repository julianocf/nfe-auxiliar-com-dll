using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Schema;
using System.IO;

namespace NFeAuxiliar
{
    //define o tipo de interface da classe
    [ClassInterface(ClassInterfaceType.AutoDual)]
    //registra um identificador para a classe no registro
    [ProgId("NFeAuxiliar")]
    //faz com que todos os métodos e propriedades da classe sejam visíveis
    [ComVisible(true)]
    //registra o GUID
    [Guid("026D5F16-6814-40B2-9987-770F4F48EC5B")]

    //para registrar:
    //C:\Windows\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe "G:\Meu Drive\Protótipos\NFeAuxiliar\NFeAuxiliar\bin\Debug\NFeAuxiliar.dll"

    //for /f %%a in ('dir %windir%\Microsoft.Net\Framework\regasm.exe /s /b') do set currentRegasm = "%%a"
    //% currentRegasm % "full\path\to\your.dll" / options
    //cd %windir%\Microsoft.Net\Framework\ 
    //cd v4.0.30319 
    //RegAsm.exe "D:\Visual Studio\repos\NFeAuxiliar\NFeAuxiliar\bin\Debug\NFeAuxiliar.dll" /tlb

    public class NFeAuxiliar
    {

        X509Certificate2 SelectedCertificate = null;

        public bool SelecionarCertificado()
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection SelectedCertificates = X509Certificate2UI.SelectFromCollection(store.Certificates, "NFeAuxiliar", "Selecione o certificado:", X509SelectionFlag.SingleSelection);
                if (SelectedCertificates.Count > 0)
                {
                    X509Certificate2Enumerator enumerator = SelectedCertificates.GetEnumerator();
                    enumerator.MoveNext();
                    SelectedCertificate = enumerator.Current;
                    store.Close();
                    return true;
                }
            }
            finally
            {
                store.Close();
            }
            return false;
        }
        public bool SelecionarCertificadoPorDigital(string digitalDoCertificado)
        {
            if (digitalDoCertificado.Length > 0)
            {
                // Get the certificate store for the current user.
                X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                try
                {
                    store.Open(OpenFlags.ReadOnly);

                    X509Certificate2Collection certCollection = store.Certificates;
                    X509Certificate2Collection certificate = certCollection.Find(X509FindType.FindByThumbprint, digitalDoCertificado, false);

                    if (certificate.Count > 0)
                    {
                        SelectedCertificate = certificate[0];
                        store.Close();
                        return true;
                    }
                }
                finally
                {
                    store.Close();
                }
            }
            return false;
        }
        public string RetornarNomeAmigavelDoCertificado()
        {
            return SelectedCertificate.FriendlyName;
        }
        public string RetornarNomeComumDoCertificado()
        {
            return SelectedCertificate.GetNameInfo(X509NameType.SimpleName, false);
        }
        public string RetornarDigitalDoCertificado()
        {
            return SelectedCertificate.Thumbprint;
        }
        public DateTime RetornarValidadeDoCertificado()
        {
            return SelectedCertificate.NotAfter.Date;
        }
        public string RetornarSerialDoCertificado()
        {
            return SelectedCertificate.SerialNumber;
        }
        public bool RetornarSeCertificadoTemChavePrivada()
        {
            return SelectedCertificate.HasPrivateKey;
        }

        public bool ValidarArquivoXML(string arquivoXml, string urlNameSpace, string arquivoSchemaXml, ref string mensagemDoErro)
        {
            // zera a mensagem de erro
            mensagemDoErro = "";
            // Carrega o arquivo XML
            XmlDocument document = new XmlDocument();
            document.Load(arquivoXml);
            // Valida o conteúdo do xml
            return ValidarStringXML(document.OuterXml, urlNameSpace, arquivoSchemaXml, ref mensagemDoErro);
        }

        // https://docs.microsoft.com/en-us/dotnet/standard/data/xml/validating-an-xml-document-in-the-dom
        public bool ValidarStringXML(string conteudoDoXml, string urlNameSpace, string arquivoSchemaXml, ref string mensagemDoErro)
        {
            try
            {
                mensagemDoErro = "";
                // Carrega as configurações de validação
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add(urlNameSpace, arquivoSchemaXml);
                settings.ValidationType = ValidationType.Schema;
                // Lê o XML
                XmlReader reader = XmlReader.Create(new StringReader(conteudoDoXml), settings);
                // Percorre o XML efetuando a validação
                while (reader.Read()) { }
            }
            catch (XmlException ex)
            {
                mensagemDoErro += String.Format("XmlDocumentValidation.XmlException: {0}", ex.Message);
                return false;
            }
            catch (XmlSchemaValidationException ex)
            {
                mensagemDoErro += String.Format("XmlDocumentValidation.XmlSchemaValidationExceptio: {0}", ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                mensagemDoErro += String.Format("XmlDocumentValidation.Exception: {0}", ex.Message);
                return false;
            }
            return true;
        }

        public string AssinarXML(string xmlDaNota, string referenceUri, string digitalDoCertificado, ref string mensagemDeRetorno)
        {
            // seleciona o certificado
            if (SelecionarCertificadoPorDigital(digitalDoCertificado))
            {
                try
                {

                    // Create a new XML document.
                    XmlDocument xmlDoc = new XmlDocument();

                    // Load an XML file into the XmlDocument object.
                    xmlDoc.PreserveWhitespace = false;
                    xmlDoc.LoadXml(xmlDaNota);

                    // valida os argumentos
                    if (xmlDoc == null)
                        throw new ArgumentException(nameof(xmlDoc));
                    if (SelectedCertificate.PrivateKey == null)
                        throw new ArgumentException(nameof(SelectedCertificate.PrivateKey));
                    if (xmlDoc.GetElementsByTagName("infNFe").Count == 0)
                        throw new ArgumentException("XML inválido ou corrompido [infNFe]");
                    if (string.IsNullOrEmpty(referenceUri))
                        throw new ArgumentException("O parâmetro referenceURI não foi informado [referenceURI]");

                    // Create a SignedXml object.
                    SignedXml signedXml = new SignedXml(xmlDoc);

                    // Add the key to the SignedXml document.
                    signedXml.SigningKey = SelectedCertificate.PrivateKey;

                    // Create a reference to be signed.
                    Reference reference = new Reference();
                    reference.Uri = "#" + referenceUri;

                    // Create a new KeyInfo object
                    KeyInfo keyInfo = new KeyInfo();

                    // Load the certificate into a KeyInfoX509Data object
                    // and add it to the KeyInfo object.
                    keyInfo.AddClause(new KeyInfoX509Data(SelectedCertificate));

                    // Add the KeyInfo object to the SignedXml object.
                    signedXml.KeyInfo = keyInfo;

                    // Add an enveloped transformation to the reference.
                    XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                    reference.AddTransform(env);

                    XmlDsigC14NTransform c14 = new XmlDsigC14NTransform();
                    reference.AddTransform(c14);

                    // Add the reference to the SignedXml object.
                    signedXml.AddReference(reference);

                    // Compute the signature.
                    signedXml.ComputeSignature();

                    // Get the XML representation of the signature and save
                    // it to an XmlElement object.
                    XmlElement xmlDigitalSignature = signedXml.GetXml();

                    // Append the element to the XML document.
                    xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));

                    return xmlDoc.InnerXml;
                }
                catch (Exception e)
                {
                    mensagemDeRetorno = e.Message;
                    return "";
                }
            }
            mensagemDeRetorno = "falha ao selecionar o certificado para a assinatura";
            return "";
        }
    }
}
