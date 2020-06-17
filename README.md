# nfe-auxiliar-com-dll
Projeto de DLL em C# para acesso a funções da API do windows via comunicação COM

Utilizado como base o projeto "HarbourCSharpCertificates"

https://github.com/andrelccorrea/csharp-certificates-para-harbour


-Funções implantadas:

	SelecionarCertificado() -> retorna booleano true se selecionou corretamente o certificado
	SelecionarCertificadoPorDigital(string digitalDoCertificado) -> retorna booleano true se selecionou corretamente o certificado
	RetornarNomeAmigavelDoCertificado() -> retorna o nome amigável do certificado em formato string
	RetornarNomeComumDoCertificado() -> retorna o nome comum do certificado em formato string
	RetornarDigitalDoCertificado() -> retorna a digital de identificação do certificado em formado string
	RetornarValidadeDoCertificado() -> retorna a validade do certificado selecionado em formato data
	RetornarSerialDoCertificado() -> retorna o serial do certificado em formato string
	RetornarSeCertificadoTemChavePrivada() -> retorna se o certificado possui chave privada
	ValidarArquivoXML(string arquivoXml, string urlNameSpace, string arquivoSchemaXml, ref string mensagemDoErro) -> valida o arquivo xml com o schema informado, retorna bool true ou false
	ValidarStringXML(string conteudoDoXml, string urlNameSpace, string arquivoSchemaXml, ref string mensagemDoErro) -> valida a string xml com o schema informado, retorna bool true ou false
	AssinarXML(string xmlDaNota, string referenceUri, string digitalDoCertificado, ref string mensagemDeRetorno) -> assina a string xml com o certificado selecionado, retorna a string do xml com a assinatura
