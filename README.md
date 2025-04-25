### TrabalhoPraticoTecnicas1 ###
O jogo escolhido foi o jogo snake feito em monogame.  
O repositorio do jogo original encontra-se [aqui](https://github.com/jasmine-blush/monogame_snake/tree/main)  

# Snake #
A classe Snake possui uma estrutura, BodyPart, que é responsável por inicializar uma nova parte do corpo da cobra numa posição (x,y) e uma direção inicial, por exemplo 0-cima ; 1-direita ; 2-baixo ; 3-esquerda. 

Possui um construtor que define o tamanho do grid, inicializa a direção inicial da cobra para a direita, ou seja 1. Cria o corpo da cobra com 3 segmentos e define a velocidade inicial da cobra e o contador de movimento.

Tem o método LoadContent, responsável por carregar as texturas da cobra, a cabeça da cobra, o corpo da cobra e a cauda da cobra.

Possui o método Update, que tem a função de armazenar qual foi a última tecla pressionada pelo jogador, reduz o contador _movementCounter com base no tempo decorrido e quando o contador atinge 0 atualiza a cabeça da cobra, com base na última tecla pressionada, consequentemente move cada segmento do corpo na direção correspondente à tecla pressionada. Deteta também se acontecem colisões, com as bordas do grid e também com o próprio corpo da cobra. Por fim, verifica se a cabeça da cobra coincide com a bola, se isso se verificar faz com que o corpo da cobra cresça um segmento e diminui o intervalo entre movimentos da cobra em 0.1, impondo um limite de 0.2, para evitar que a cobra se mova muito rápido.

Existe o método Draw, responsável por desenhar a cabeça da cobra, com a rotação certa, realizando o mesmo processo para o corpo e a cauda da cobra.

Por fim existe o método GetMovement, responsável por retornar um vetor de movimento com base na direção fornecida, ou seja, 0: Movimento para cima (0, -1) ; 1: Movimento para a direita (1, 0) ; 2: Movimento para baixo (0, 1) ; 3: Movimento para a esquerda (-1, 0).

# Ball Spawner #

BallPosition: Representa a posição atual da bola na grid, utilizando um vetor 2D (Vector2).

_gridSize: Define o tamanho da grade (linhas, colunas).

_rng: Objeto da classe Random, usado para gerar posições aleatórias.

_ballTexture: Textura da bola, carregada a partir dos recursos do jogo.

Esta classe possui um contrutor "BallSpawner" que utiliza a func "Random" para atribuir a _rng um valor aleatório onde a bola será renderizada, este valor vai ser delimitado pelo tamanho do campo 
"_gridsize" atribuindo-lhe os valores de "gridSize" que serão inicializados na classe "Game1", e chama a função "SpawnNew" para posicionar a bola aleatoriamente.

Tem uma função "LoadContent" que carrega o sprite da Bola, guardando-o em "_balltexture", através da função ContentManager para carregar a textura "ball" e o parâmetro content para gerir o conteúdo usado para carregar a textura.

O método Draw usa o parâmetro "spritebatch" para renderizar a bola na posição atual, "BallPosition", que será centralizada na grid com a ajuda de um deslocamento: "new Vector2(20, 20) + new Vector2(2f, 2f)" e tomará a cor normal do sprite visto que "Color.White" usa a cor original do sprite, sem alterações.

Por fim tem a função SpawnNew que usa o "_rng" para gerar um novo valor aleatório para "Ballposition" que está limitado pelo "_gridSize"

 
