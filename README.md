FBRPan
========

Único painel de emulador Open Source feito em Socket e HTML para você!

##Configurando o server:
No arquivo principal você pode modificar as variaveis. Tais como o website em que se encontra o arquivo de login e os tipos de emuladores que serão usados.
Para configurar o login, você pode fazer da forma que desejar. Este painel está configurado para enviar um request com o usuário e senha do emulador para a página login.php e retornar o nome do emulador local, o login será efetuado.

Exemplo: 
  
    GET login.php?user=bluhhotel&pass=123456
    RESPONSE: bluhemulator

No exemplo, o login foi efetuado usando o usuário bluhhotel e senha 123456. O nome da pasta local do emulador configurado é bluhemulator
Se a resposta do arquivo login for 0, o login é rejeitado.
O arquivo de login se encontra junto com os arquivos web no arquivo web.zip (login.php deve ser configurado de tal modo que com os requests retorne o nome da pasta do emulador, não iremos passar database alguma para o login)

Para mais configurações, aprenda na source!

##Contribuições
Somente serão aceitas contribuições realmente úteis para o funcionamento do painel e que contenham todas as informações do que foi alterado!
