### TrabalhoPraticoTecnicas1 ###
O jogo escolhido foi o jogo snake feito em monogame.  
O repositorio do jogo original encontra-se [aqui](https://github.com/jasmine-blush/monogame_snake/tree/main)  

# Content #

Neste jogo é necessário como contéudo uma imagem do tipo .png para a cabeça da cobra, para o corpo da cobra e para a cauda, também é utilizada uma imagem para a grade ("Grid") onde o jogo acontece e uma imagem para a bola. Finalemnte, também é ncessario o o font da letra que é utilizado para o game over.

# Game Over #  
O jogo termina quando a cabeça da cobra bate no corpo dela ou na fronteira da grid, caso isso aconteça apararece escrito on ecrâ "Game Over" e o jogo termina.

```
 //Generate Game Over screen as a texture
            _font = Content.Load<SpriteFont>("arial");
            Vector2 gameOverSize = _font.MeasureString("Game Over!");
            _gameOverTexture = new RenderTarget2D(
                _graphics.GraphicsDevice,
                (int)Math.Ceiling(gameOverSize.X),
                (int)Math.Ceiling(gameOverSize.Y)
            );

            GraphicsDevice.SetRenderTarget(_gameOverTexture);
            GraphicsDevice.Clear(Color.Transparent);
            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, "Game Over!", Vector2.Zero, Color.Red);
            _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            //-----
O jogo funciona maioritariamente com duas classes sendo estas Snake e Ball Spawner.
```
O código abaixo é onde o na classe snake ele verifica e chama o Game1 para o game over acontecer.
```
        if (i == 0)
                    {
                        if (
                            part.Position.X < 0
                            || part.Position.Y < 0
                            || part.Position.X >= _gridSize.Item1
                            || part.Position.Y >= _gridSize.Item2
                        )
                        {
                            Game1.GameOver = true;
                            break;
                        }
```
# Snake #
A classe Snake possui uma estrutura, BodyPart, que é responsável por inicializar uma nova parte do corpo da cobra numa posição (x,y) e uma direção inicial, por exemplo 0-cima ; 1-direita ; 2-baixo ; 3-esquerda. 

```
   internal struct BodyPart
    {
        public BodyPart(float x, float y, int direction)
        {
            Position = new Vector2(x, y);
            Direction = direction;
        }
```

Possui um construtor que define o tamanho do grid, inicializa a direção inicial da cobra para a direita, ou seja 1. Cria o corpo da cobra com 3 segmentos e define a velocidade inicial da cobra e o contador de movimento.

```
   public Snake((int, int) gridSize)
        {
            _gridSize = gridSize;
            _direction = 1;
            _body = new List<BodyPart>
            {
                new BodyPart(2, 0, _direction),
                new BodyPart(1, 0, _direction),
                new BodyPart(0, 0, _direction)
            };
            _snakeSpeed = 1.0f;
            _movementCounter = MOVEMENT_COUNTER;
        }
```

Tem o método LoadContent, responsável por carregar as texturas da cobra, a cabeça da cobra, o corpo da cobra e a cauda da cobra.
```
 public void LoadContent(ContentManager content)
        {
            _headTexture = content.Load<Texture2D>("snake_head");
            _bodyTexture = content.Load<Texture2D>("snake_body");
            _tailTexture = content.Load<Texture2D>("snake_tail");
        }
```
Possui o método Update, que tem a função de armazenar qual foi a última tecla pressionada pelo jogador, reduz o contador _movementCounter com base no tempo decorrido e quando o contador atinge 0 atualiza a cabeça da cobra, com base na última tecla pressionada, consequentemente move cada segmento do corpo na direção correspondente à tecla pressionada. Deteta também se acontecem colisões, com as bordas do grid e também com o próprio corpo da cobra. Por fim, verifica se a cabeça da cobra coincide com a bola, se isso se verificar faz com que o corpo da cobra cresça um segmento e diminui o intervalo entre movimentos da cobra em 0.1, impondo um limite de 0.2, para evitar que a cobra se mova muito rápido.
```
public void Update(GameTime gameTime, BallSpawner ball)
        {
            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.W))
            {
                _lastKey = Keys.W;
            }

            if (kstate.IsKeyDown(Keys.S))
            {
                _lastKey = Keys.S;
            }

            if (kstate.IsKeyDown(Keys.A))
            {
                _lastKey = Keys.A;
            }

            if (kstate.IsKeyDown(Keys.D))
            {
                _lastKey = Keys.D;
            }

            _movementCounter -= gameTime.ElapsedGameTime.Milliseconds;
            if (_movementCounter <= 0)
            {
                _movementCounter = MOVEMENT_COUNTER * _snakeSpeed;

                BodyPart head = _body[0];
                if (_lastKey == Keys.W && !(head.Direction == 2))
                {
                    head.Direction = 0;
                }

                if (_lastKey == Keys.S && !(head.Direction == 0))
                {
                    head.Direction = 2;
                }

                if (_lastKey == Keys.A && !(head.Direction == 1))
                {
                    head.Direction = 3;
                }

                if (_lastKey == Keys.D && !(head.Direction == 3))
                {
                    head.Direction = 1;
                }
                _body[0] = head;

                int currDirection = head.Direction;
                for (int i = 0; i < _body.Count; i++)
                {
                    BodyPart part = _body[i];
                    part.Position += GetMovement(part.Direction);
                    if (i == 0)
                    {
                        if (
                            part.Position.X < 0
                            || part.Position.Y < 0
                            || part.Position.X >= _gridSize.Item1
                            || part.Position.Y >= _gridSize.Item2
                        )
                        {
                            Game1.GameOver = true;
                            break;
                        }
                    }
                    int nextDirection = part.Direction;
                    part.Direction = currDirection;
                    currDirection = nextDirection;
                    _body[i] = part;
                }
                for (int i = 1; i < _body.Count; i++)
                {
                    if (_body[i].Position == _body[0].Position)
                    {
                        Game1.GameOver = true;
                        break;
                    }
                }

                if (ball.BallPosition == _body[0].Position)
                {
                    ball.SpawnNew();
                    _snakeSpeed -= 0.1f;
                    if (_snakeSpeed < 0.2f)
                        _snakeSpeed = 0.2f;
                    BodyPart tail = _body[_body.Count - 1];
                    Vector2 offset = GetMovement(tail.Direction);
                    offset *= new Vector2(-1, -1);
                    Vector2 newPartPosition = tail.Position + offset;
                    _body.Add(new BodyPart(newPartPosition.X, newPartPosition.Y, tail.Direction));
                }
            }
        }
```
Existe o método Draw, responsável por desenhar a cabeça da cobra, com a rotação certa, realizando o mesmo processo para o corpo e a cauda da cobra.
```
 public void Draw(SpriteBatch spriteBatch)
        {
            foreach (BodyPart part in _body)
            {
                if (_body.IndexOf(part) == 0)
                {
                    spriteBatch.Draw(
                        _headTexture,
                        part.Position * new Vector2(20, 20) + new Vector2(10, 10),
                        null,
                        Color.White,
                        MathHelper.ToRadians(90 * part.Direction),
                        new Vector2(_headTexture.Width / 2, _headTexture.Height / 2),
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
                else if (_body.IndexOf(part) == _body.Count - 1)
                {
                    spriteBatch.Draw(
                        _tailTexture,
                        part.Position * new Vector2(20, 20) + new Vector2(10, 10),
                        null,
                        Color.White,
                        MathHelper.ToRadians(90 * part.Direction),
                        new Vector2(_tailTexture.Width / 2, _tailTexture.Height / 2),
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
                else
                {
                    spriteBatch.Draw(
                        _bodyTexture,
                        part.Position * new Vector2(20, 20) + new Vector2(10, 10),
                        null,
                        Color.White,
                        MathHelper.ToRadians(90 * part.Direction),
                        new Vector2(_bodyTexture.Width / 2, _bodyTexture.Height / 2),
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
            }
        }
```
Por fim existe o método GetMovement, responsável por retornar um vetor de movimento com base na direção fornecida, ou seja, 0: Movimento para cima (0, -1) ; 1: Movimento para a direita (1, 0) ; 2: Movimento para baixo (0, 1) ; 3: Movimento para a esquerda (-1, 0).
```
  #region Utility
        private static Vector2 GetMovement(int direction)
        {
            return direction switch
            {
                0 => new Vector2(0, -1),
                1 => new Vector2(1, 0),
                2 => new Vector2(0, 1),
                3 => new Vector2(-1, 0),
                _ => new Vector2(0, 0),
            };
        }

        #endregion
    }

```
# Ball Spawner #

BallPosition: Representa a posição atual da bola na grid, utilizando um vetor 2D (Vector2).

_gridSize: Define o tamanho da grid (linhas, colunas).

_rng: Objeto da classe Random, usado para gerar posições aleatórias.

_ballTexture: Textura da bola, carregada a partir dos recursos do jogo.
```
 internal class BallSpawner
    {
        public Vector2 BallPosition;
        private (int, int) _gridSize;
        private Random _rng;
        private Texture2D _ballTexture;
```

Esta classe possui um contrutor "BallSpawner" que utiliza a func "Random" para atribuir a _rng um valor aleatório onde a bola será renderizada, este valor vai ser delimitado pelo tamanho do campo 
"_gridsize" atribuindo-lhe os valores de "gridSize" que serão inicializados na classe "Game1", e chama a função "SpawnNew" para posicionar a bola aleatoriamente.
```
public BallSpawner((int, int) gridSize)
        {
            _rng = new Random();
            _gridSize = gridSize;
            SpawnNew();
        }
```
Tem uma função "LoadContent" que carrega o sprite da Bola, guardando-o em "_balltexture", através da função ContentManager para carregar a textura "ball" e o parâmetro content para gerir o conteúdo usado para carregar a textura.
```
 public void LoadContent(ContentManager content)
        {
            _ballTexture = content.Load<Texture2D>("ball");
        }
```
O método Draw usa o parâmetro "spritebatch" para renderizar a bola na posição atual, "BallPosition", que será centralizada na grid com a ajuda de um deslocamento: "new Vector2(20, 20) + new Vector2(2f, 2f)" e tomará a cor normal do sprite visto que "Color.White" usa a cor original do sprite, sem alterações.
```
 public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _ballTexture,
                BallPosition * new Vector2(20, 20) + new Vector2(2f, 2f),
                Color.White
            );
        }
```
Por fim tem a função SpawnNew que usa o "_rng" para gerar um novo valor aleatório para "Ballposition" que está limitado pelo "_gridSize".
```
 public void SpawnNew()
        {
            BallPosition = new Vector2(
                _rng.NextInt64(_gridSize.Item1),
                _rng.NextInt64(_gridSize.Item2)
            );
        }
 ```


# Críticas #

Apesar de ser baseado num jogo muito simples que nós todos já jogámos, ainda tem algumas falhas.

- É preciso fechar o programa e voltar a abrir para jogar de novo.
- As paredes acabam o Jogo em vez de servirem como "Portal" para a parede oposta.
- O som está muito alto e não há forma de alterar.
- A bola pode ser renderizada dentro da cobra
