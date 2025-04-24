### TrabalhoPraticoTecnicas1 ###
O jogo escolhido foi TheGreen que é um jogo com mecanicas semelhantes a Terraria, sendo estas a geração de um mundo 2D na qual o jogador pode interagir cortando arvores, minerando e construindo estruturas.
#Classes a falar#
- WorldGeneration
- Lighting
- Entities
- Inventory

## WorldGen ##
Na classe WorldGen é onde o mundo é gerado, guardado e onde carregado caso esta não seja a primeira vez que jogue. Tambem é a classe principal da pasta WorldGeneration de onde varias outras classes relacionadas com o mundo interagem.  
#Para a geração do mundo são utilizadas as seguintes etapas#  
Etapas de Geração
1.Inicialização:

Define o tamanho do mundo (WorldSize) e inicializa o array de tiles.
Configura o gerador de números aleatórios com a semente fornecida (ou gera uma aleatória).
2.Superfície do Terreno:

Usa o método Generate1DNoise para criar um ruído que determina as elevações do terreno.
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
#Métodos Auxiliares#
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
#Atualizações no Mundo#
InitializeGameUpdates:

Configura atualizadores do mundo, como:
LiquidUpdater: Gerencia o fluxo de líquidos.
OverlayTileUpdater: Gerencia tiles de sobreposição, como grama.
Update:

Executa atualizações periódicas em líquidos, grama e tiles danificados.
#Manipulação de Tiles#
SetTile, RemoveTile:

Define ou remove tiles no mundo, atualizando estados e propriedades.
SetTileState, UpdateWallState:

Calcula e aplica estados para tiles e paredes com base nos vizinhos.
GenerateTree:

Adiciona árvores ao mundo, gerando tronco e topo.
#Guardar e Carregamento#
SaveWorld, LoadWorld:
Salva ou carrega o estado do mundo em arquivos binários.
Inclui informações sobre os tiles, paredes, líquidos, e inventários.
#Interatividade#
DamageTile:

Aplica dano a um tile. Se a saúde do tile for reduzida a 0, ele é removido.
GetTileInventory, AddTileInventory:

Gerencia inventários associados a tiles, como baús.

## Inventory ##


