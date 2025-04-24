### TrabalhoPraticoTecnicas1 ###
O jogo escolhido foi TheGreen que é um jogo com mecanicas semelhantes a Terraria, sendo estas a geração de um mundo 2D na qual o jogador pode interagir cortando arvores, minerando e construindo estruturas.
#Classes a falar#
- WorldGeneration
- Lighting
- Entities
- Inventory

## WorldGen ##
Na classe WorldGen é onde o mundo é gerado, guardado e onde carregado caso esta não seja a primeira vez que jogue. Tambem é a classe principal da pasta WorldGeneration de onde varias outras classes relacionadas com o mundo interagem.  
Para a geração do mundo são utilizadas as seguintes etapas  
Etapas de Geração  
1.Inicialização:

Define o tamanho do mundo (WorldSize) e inicializa o array de tiles.
Configura o gerador de números aleatórios com a semente fornecida (ou gera uma aleatória).
2.Superfície do Terreno:

Usa a função Generate1DNoise para criar um ruído que determina as elevações do terreno.
Suaviza o ruído com Smooth para reduzir transições abruptas.  
Coloca tiles de pedra e calcula a altura da superfície.  
3.Adição de Camadas:  

Adiciona camadas de terra e grama abaixo da superfície.  
4.Geração de Cavernas:  

Usa ruído Perlin bidimensional (GeneratePerlinNoiseWithOctaves) para determinar onde cavernas serão formadas.  
Remove tiles com base nos valores do ruído.  
5.Cálculo de Estados:  

Atualiza o estado de cada tile (SetTileState) e parede (UpdateWallState) para garantir consistência.  
6.Espalhamento de Grama e Plantação de Árvores:  

Espalha grama sobre a superfície.  
Planta árvores em posições aleatórias, respeitando distâncias mínimas entre elas.  
Métodos Auxiliares  
Generate1DNoise:  

Gera um vetor de ruído 1D para determinar a elevação do terreno.  
Aplica interpolação Catmull-Rom para suavizar transições.  
GeneratePerlinNoiseWithOctaves:  

Cria um mapa de ruído bidimensional para cavernas.  
Usa múltiplas frequências e amplitudes para adicionar detalhes.  
Smooth:  

Suaviza os valores de ruído calculando a média de um valor e seus vizinhos.  
InitializeGradients:  

Inicializa gradientes aleatórios para uso no cálculo de ruído Perlin.  
Atualizações no Mundo  
InitializeGameUpdates:  

Configura atualizadores do mundo, como:  
LiquidUpdater: Gerencia o fluxo de líquidos.  
OverlayTileUpdater: Gerencia tiles de sobreposição, como grama.  
Update:  

Executa atualizações periódicas em líquidos, grama e tiles danificados.  
Manipulação de Tiles  
SetTile, RemoveTile:  

Define ou remove tiles no mundo, atualizando estados e propriedades.  
SetTileState, UpdateWallState:  

Calcula e aplica estados para tiles e paredes com base nos vizinhos.  
GenerateTree:  

Adiciona árvores ao mundo, gerando tronco e topo.  
#Guardar e Carregamento  
SaveWorld, LoadWorld:  
Salva ou carrega o estado do mundo em arquivos binários.  
Inclui informações sobre os tiles, paredes, líquidos, e inventários.  
#Interatividade  
DamageTile:  

Aplica dano a um tile. Se a saúde do tile for reduzida a 0, ele é removido.  
GetTileInventory, AddTileInventory:  

Gerencia inventários associados a tiles, como baús.

## Inventory ##

## Light Engine ##
O LightEngine.cs implementa o sistema de iluminação para o jogo. Este componente é responsável por calcular e aplicar os efeitos de luz no mundo do jogo, considerando luzes estáticas e dinâmicas, absorção de luz por diferentes materiais, e propagação de luz para simular iluminação realista.

Funcionalidades:
Mapa de Luz (_lightMap): Estrutura que armazena as propriedades de luz (intensidade e máscara de absorção) para cada ponto do mundo do jogo.

Luzes Dinâmicas(_dynamicLights): Permite adicionar luzes que variam frame a frame, como luzes de tochas ou explosões.

Propagação de Luz: Implementação de propagação de luz em múltiplas direções para simular efeitos de iluminação difusa.

Absorção de Luz: Define como diferentes tipos de materiais (paredes, blocos, líquidos) absorvem ou transmitem a luz.

Suporte a Áreas Visíveis: Cálculo otimizado dentro de uma área delimitada pela visão do jogador, considerando um buffer adicional para o alcance da luz.

Métodos:
CalculateLightMap: Método central que executa três passos principais:

	Limpa o mapa de luz aplicando absorção e valores padrão.
	Aplica as luzes dinâmicas adicionadas pela lógica do jogo.
	Realiza a propagação da luz em todo o mapa.

ClearLightMap: Inicializa o mapa de luz, determinando valores de luz e absorção com base em elementos do mundo como blocos sólidos, paredes, líquidos e blocos emissores de luz.

ApplyDynamicLights: Processa e aplica as luzes dinâmicas adicionadas durante o jogo.

SpreadLight: Propaga a luz no mapa, suavizando os efeitos de iluminação para criar uma distribuição realista.

SetDrawBox: Define os limites da área do mapa onde a luz será calculada, considerando o alcance da luz.

GetLight: Retorna a cor da luz em um ponto específico do mapa.

AddLight: Permite adicionar uma luz dinâmica em uma posição específica.

Estrutura de Dados
Absorção de Materiais:

Paredes (_wallAbsorption): Absorvem 90% da luz.
Blocos (_tileAbsorption): Absorvem 70% da luz.
Líquidos (_liquidLightAbsorption): Absorvem luz com valores específicos para cada canal de cor (R, G, B).
Fila de Luzes Dinâmicas (_dynamicLights): Estrutura que armazena luzes dinâmicas para serem processadas.

Uso
Configuração Inicial:

Instanciar a classe LightEngine, passando o dispositivo gráfico do XNA como argumento.
Configurar os limites da área visível com SetDrawBox.
Atualização por Frame:

Adicionar luzes dinâmicas com AddLight.
Calcular o mapa de luz para o frame atual usando CalculateLightMap.
Consulta de Luz:

Utilizar GetLight para obter a cor da luz em posições específicas do mapa.
