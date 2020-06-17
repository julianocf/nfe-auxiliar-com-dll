# # nfe-auxiliar-com-dll
Projeto de DLL em C# para acesso a funções da API do windows via comunicação COM

Utilizado como base o projeto "HarbourCSharpCertificates"

https://github.com/andrelccorrea/csharp-certificates-para-harbour

# Funções implantadas:
 - SelecionarCertificado() -> retorna booleano true se selecionou corretamente o certificado
 - SelecionarCertificadoPorDigital(string digitalDoCertificado) -> retorna booleano true se selecionou corretamente o certificado
 - RetornarNomeAmigavelDoCertificado() -> retorna o nome amigável do certificado em formato string
 - RetornarNomeComumDoCertificado() -> retorna o nome comum do certificado em formato string
 - RetornarDigitalDoCertificado() -> retorna a digital de identificação do certificado em formado string
 - RetornarValidadeDoCertificado() -> retorna a validade do certificado selecionado em formato data
 - RetornarSerialDoCertificado() -> retorna o serial do certificado em formato string
 - RetornarSeCertificadoTemChavePrivada() -> retorna se o certificado  possui chave privada
 - ValidarArquivoXML(string arquivoXml, string urlNameSpace, string arquivoSchemaXml, ref string mensagemDoErro) -> valida o arquivo xml com o schema informado, retorna bool true ou false
 - ValidarStringXML(string conteudoDoXml, string urlNameSpace, string  arquivoSchemaXml, ref string mensagemDoErro) -> valida a string xml com o schema informado, retorna bool true ou false
 - AssinarXML(string xmlDaNota, string referenceUri, string digitalDoCertificado, ref string mensagemDeRetorno) -> assina a string xml com o certificado selecionado, retorna a string do xml com a assinatura

# Registrando a DLL

 - Como administrador, executar o seguinte comando no prompt de comando do Windows:
 - `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe "Diretorio_da_dll\NFeAuxiliar.dll"`

# Uso com Harbour

Criando o objeto:

    TRY
      dll_nfe_auxiliar := GetActiveObject( "NFeAuxiliar" )
    CATCH
      TRY
        dll_nfe_auxiliar := CreateObject( "NFeAuxiliar" )
      CATCH
        MsgAlert( Ole2TxtError() )
      END
    END

Chamando uma função:

    dll_nfe_auxiliar:SelecionarCertificado()

