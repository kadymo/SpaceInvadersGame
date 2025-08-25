# 📖 Introdução
Este documento descreve o processo de estruturação e desenvolvimento do Projeto Capstone: Space Invaders, desenvolvido na disciplina de Programação III. O objetivo deste jogo é reforçar e aprimorar o conhecimento teórico e prático em fundamentos da linguagem C#, entendendo recursos importantes oferecidos pela linguagem como desenvolvimento desktop com o Uno Platform, events, delegates, data binding, canvas component, disparo de audio, etc.

# 🎯 Objetivo e Escopo

O objetivo do jogo é fornecer um meio de entretenimento interativo, divertido, otimizado e acessível para qualquer pessoa jogar. O escopo de funcionalidades do projeto inclui todos os principais movimentos e ações de todos os personagens (jogador, inimigos) ou componentes (barreiras, projéteis) fundamentais para a dinâmica e fluxo do jogo. Ao final do desenvolvimento, o jogador é capaz de usufruir de uma experiência completa de um jogo desktop baseado no Space Invaders, interagindo e disparando eventos via teclado, recebendo feedbacks visuais e sonoros, e se movimentando de maneira fluida no jogo.

# 🛠️ Tecnologias e Ferramentas

| Tecnologia                                                                                                                  | Descrição                                                                           |
|-----------------------------------------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------|
| <img src="https://upload.wikimedia.org/wikipedia/commons/b/bd/Logo_C_sharp.svg" align="center" margin="20" width="20">C#    | Linguagem de programação                                                            |
| <img src="https://raw.githubusercontent.com/tandpfun/skill-icons/main/icons/DotNet.svg" align="center" width="20"> .NET 9.0 | Plataforma de desenvolvimento em C#                                                 |
| <img src="https://avatars.githubusercontent.com/u/52228309?s=280&v=4" width="20" align="center">Uno Platform                | Plataforma para a criação de aplicações baseadas em interface de usuário |

# ️📱 Requisitos

## Requisitos Funcionais

<table>
    <tr>
        <th>Identificador</th>
        <th>Requisito</th>
    </tr>
    <tr>
        <th colspan="2" align="center">Jogador</th>
    </tr>
    <tr>
        <td>RF-01</td>
        <td>O jogador será capaz de se mover da esquerda para a direita, pressionando as setas para esquerda e direita, respectivamente</td>
    </tr>
    <tr>
        <td>RF-02</td>
        <td>O botão espaço (space) será usado para atacar</td>
    </tr>
    <tr>
        <td>RF-03</td>
        <td>O jogador pode disparar novamente quando seu tiro anterior acertar ou de outra forma quando exceder o limite superior do quadro do jogo</td>
    </tr>
    <tr>
        <th colspan="2" align="center">Naves alienígenas</th>
    </tr>
    <tr>
        <td>RF-04</td>
        <td>Ela se move da esquerda para a direita, uma vez que atinge a borda, em seguida, ela se move para baixo uma posição e se move para a borda oposta</td>
    </tr>
    <tr>
        <td>RF-05</td>
        <td>Cada vez que há um rolo para baixo, a velocidade do movimento aumenta um pouco, assim como a velocidade do fogo deles</td>
    </tr>
    <tr>
        <td>RF-06</td>
        <td>A única nave que ataca é o que vale 40 PTS</td>
    </tr>
    <tr>
        <td>RF-07</td>
        <td>Os blocos de proteção se desgastam toda vez que um tiro é atingido, seja de uma nave alienígena ou do jogador</td>
    </tr>
    <tr>
        <td>RF-08</td>
        <td>Alienígenas não podem colidir com os blocos de proteção</td>
    </tr>
    <tr>
        <td>RF-09</td>
        <td>Naves alienígenas são destruídas com 1 tiro</td>
    </tr>
    <tr>
        <td>RF-10</td>
        <td>Quando todas as naves alienígenas são destruídas, uma nova onda é gerada movendo a posição para baixo para os primeiros 5 e aumentando ligeiramente a velocidade a partir da quinta em diante</td>
    </tr>
    <tr>
        <td>RF-11</td>
        <td>A cada 1000 pontos, o número de vidas é aumentado em uma, até um máximo de 6</td>
    </tr>
    <tr>
        <th colspan="2" align="center">Blocos de Proteção</th>
    </tr>
    <tr>
        <td>RF-12</td>
        <td>Serão 4 blocos de proteção que ajudarão o jogador com os tiros dos alienígenas. Estes mudarão sua cor de branco para chumbo, à medida que mais dano eles recebem dos tiros alienígenas, eventualmente desaparecendo</td>
    </tr>
    <tr>
        <th colspan="2" align="center">Quando o jogo termina</th>
    </tr>
    <tr>
        <td>RF-13</td>
        <td>O jogo terminará quando as vidas do jogador acabarem</td>
    </tr>
    <tr>
        <td>RF-14</td>
        <td>Ou acabará quando as naves alienígenas alcançarem o jogador</td>
    </tr>
    <tr>
        <th colspan="2" align="center">Pontuação</th>
    </tr>
    <tr>
        <td>RF-15</td>
        <td>A pontuação é exibida no canto superior esquerdo</td>
    </tr>
    <tr>
        <td>RF-16</td>
        <td>Ela é incrementada seguindo a tabela mostrada acima cada vez que um alienígena é destruído</td>
    </tr>
    <tr>
        <th colspan="2" align="center">O que acontece após o fim do jogo</th>
    </tr>
    <tr>
        <td>RF-17</td>
        <td>O jogador tem a opção de salvar sua pontuação adicionando um apelido</td>
    </tr>
    <tr>
        <td>RF-18</td>
        <td>O jogador tem a opção de voltar a jogar</td>
    </tr>
    <tr>
        <td>RF-19</td>
        <td>Se o jogador não quiser jogar novamente, a página inicial será exibida</td>
    </tr>
    <tr>
        <th colspan="2" align="center">Tela Inicial</th>
    </tr>
    <tr>
        <td>RF-20</td>
        <td>A tela inicial dará a opção de iniciar um novo jogo</td>
    </tr>
    <tr>
        <td>RF-21</td>
        <td>A tela inicial dará a opção de ver a tabela com os placares</td>
    </tr>
    <tr>
        <td>RF-22</td>
        <td>A tela inicial dará a opção de os controles do jogo</td>
    </tr>
    <tr>
        <td colspan="2" align="center">Outros Detalhes</td>
    </tr>
    <tr>
        <td>RF-23</td>
        <td>Cada ação no jogo deve ser acompanhada por um som representativo</td>
    </tr>
    <tr>
        <td>RF-24</td>
        <td>As informações do painel de pontuação devem ser salvas em um arquivo de texto</td>
    </tr>
</table>

## Requisitos Não Funcionais

| Identificador | Tipo                     | Requisito |
|---------------|--------------------------|-----------|
| RNF-01        | Desempenho e Performance | O jogo e todos os seus componentes (jogador, inimigos, projéteis) devem ser otimizados e executados de maneira fluida, sem impacto no desempenho para o usuário         |
| RNF-02        | Acessibilidade           | O jogo deve ser acessível para todos os tipos de usuário, fornecendo feedback para cada ação executada|
| RNF-03        | Usabilidade              | O jogo deve ser simples e fácil de interagir, com eventos de teclado em teclas tradicionais e feedback visual claro|
| RNF-04        | Compatibilidade | O jogo deve ser compatível para diferentes sistemas operacionais (Windows, Linux, MacOS) |

## 📂 Estrutura de Diretórios

- Assets:
  - Arquivos de fontes, imagens e sons
- Models:
  - Definição das entidades do jogo, incluindo personagens e componentes (`Player`, `Enemy`, `Obstacle`, etc.)
- Presentation:
  - Responsável apenas pela renderização na interface gráfica através do Canvas
  - Não lida com nada relacionado à lógica do jogo
- Services:
  - Lida com tudo relacionado à lógica das interações dentro do jogo, como movimentações, disparos, colisões, etc.
- ViewModels:
  - Responsável por dizer à camada de Presentation o que deve ser renderizado no Canvas
  - Cria os objetos do jogo armazenados em `GameObjects`

  - Trabalha como uma ponte entre o Presentation, Services e Models
````
.
└── SpaceInvadersGame/
    ├── Assets/
    │   ├── Fonts/
    │   ├── Images/
    │   └── Sounds/
    ├── Models/
    │   ├── Enums/
    │   ├── Interfaces/
    │   └── ...
    ├── Presentation/
    │   ├── MainPage.xaml
    │   ├── MainPage.xaml.cs
    │   ├── MenuPage.xaml
    │   └── MenuPage.xaml.cs
    ├── Services/
    │   ├── GameManager.cs
    │   ├── InputManager.cs
    │   └── SoundManager.cs
    └── ViewModels/
        ├── MainViewModel.cs
        └── MenuViewModel.cs
````

## 🏛️ Arquitetura

O jogo foi arquitetado seguindo o Padrão MVVM (Main-View-ViewModel), prezando pela separação de responsabilidades entre as camadas e a utilização de eventos.

- A principal característica da arquitetura é a utilização de eventos
- MainPage:
  - Escuta eventos vindos de ``MainViewModel``, que notificam a criação de objetos do jogo a serem renderizados no Canvas
  - Por exemplo, o método ``OnEnemyRemoved`` em `MainPage.xaml.cs` reage ao evento `EnemyRemoved` que é disparado por `MainViewModel`
- MainViewModel:
  - Escuta eventos vindos de ``GameManager`` que são disparados quando algum objeto do jogo deve ser criado ou removido do jogo
  - Ao escutar um evento, realiza a criação ou remoção de um objeto e dispara um evento que será escutado por `MainPage`, que refletirá isso no Canvas
  - Por exemplo, o método ``OnProjectileHitEnemy`` em `MainViewModel` reage ao evento `ProjectileHitEnemy` que é disparado por `GameManager` quando um projétil atinge um inimigo
- GameManager:
  - Responsável por toda a lógica do jogo:
    - Movimentação
    - Disparos
    - Colisões
  - Dispara eventos que notificam acontecimentos do jogo e são escutados por ``MainViewModel``, que faz a criação e remoção de objetos do jogo
  - Possui um método ``Update()`` que é chamado a cada frame do jogo para executar as lógicas necessárias

<figure>
    <img width="7954"  alt="image"  src="https://github-production-user-asset-6210df.s3.amazonaws.com/98963793/481420275-83381f65-0142-4bac-ac44-123bf8af3099.svg?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAVCODYLSA53PQK4ZA%2F20250824%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250824T235413Z&X-Amz-Expires=300&X-Amz-Signature=9d89e372250c61d860040fe4f51446df93d62669f42450f9e534685b79c03117&X-Amz-SignedHeaders=host" />
    <figcaption><a href="https://github-production-user-asset-6210df.s3.amazonaws.com/98963793/481420275-83381f65-0142-4bac-ac44-123bf8af3099.svg?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAVCODYLSA53PQK4ZA%2F20250824%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250824T235413Z&X-Amz-Expires=300&X-Amz-Signature=9d89e372250c61d860040fe4f51446df93d62669f42450f9e534685b79c03117&X-Amz-SignedHeaders=host">Link da Imagem</a></figcaption>
</figure>

# 🔄️ Fluxo do Jogo

<figure>
    <img src="https://github-production-user-asset-6210df.s3.amazonaws.com/98963793/481425239-daa38736-7eff-4f53-8fd8-b026535093fa.svg?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAVCODYLSA53PQK4ZA%2F20250825%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250825T005049Z&X-Amz-Expires=300&X-Amz-Signature=4ad9c5106d5776bbb8727c1aab1589801da5d34106dfc3f3416e21e9e32c5ce9&X-Amz-SignedHeaders=host">
    <figcaption>
        <a href="https://github-production-user-asset-6210df.s3.amazonaws.com/98963793/481425239-daa38736-7eff-4f53-8fd8-b026535093fa.svg?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAVCODYLSA53PQK4ZA%2F20250825%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250825T005049Z&X-Amz-Expires=300&X-Amz-Signature=4ad9c5106d5776bbb8727c1aab1589801da5d34106dfc3f3416e21e9e32c5ce9&X-Amz-SignedHeaders=host">Link da imagem do diagrama</a>
    </figcaption>
</figure>

# 🗒️ Manual: Como jogar

| Ação                         | Tecla                        |
|------------------------------|------------------------------|
| Movimentação para a esquerda | <kbd>-></kbd> ou <kbd>D</kbd> |
| Movimentação para a difeira  | <kbd><-</kbd> ou <kbd>A</kbd> |
| Atirar um projétil           | <kbd>Space</kbd>             |

# 📸 Capturas de Tela

## Tela Inicial

<img src="https://github-production-user-asset-6210df.s3.amazonaws.com/98963793/481422763-5d625799-afc2-4a06-9a31-6f2178848042.png?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAVCODYLSA53PQK4ZA%2F20250825%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250825T002555Z&X-Amz-Expires=300&X-Amz-Signature=a80ba9ced32f609f552c1030c5695021dd6ab2ee6526f33233dc40b03f298484&X-Amz-SignedHeaders=host">

## Tela de Placares
<img src="https://github-production-user-asset-6210df.s3.amazonaws.com/98963793/481422629-729c4964-6194-4e47-97a5-424d2708d8fd.png?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAVCODYLSA53PQK4ZA%2F20250825%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250825T002427Z&X-Amz-Expires=300&X-Amz-Signature=31fadf66a213790b368913197b8d78929e669ea79dacb491bf7355b2345e62aa&X-Amz-SignedHeaders=host">

## Tela de Jogo
<img src="https://github-production-user-asset-6210df.s3.amazonaws.com/98963793/481423028-1ac1dfe8-3276-49c1-a099-69aa277d645c.png?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAVCODYLSA53PQK4ZA%2F20250825%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250825T002844Z&X-Amz-Expires=300&X-Amz-Signature=6014fb0568b0bdcc1abbfd1527b7d01a7c9cfe5577fa7cf03403299f9d678a70&X-Amz-SignedHeaders=host">

## Tela de Game Over
<img src="https://github-production-user-asset-6210df.s3.amazonaws.com/98963793/481423185-33f6bd08-851a-466a-a093-284f93f3fc11.png?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAVCODYLSA53PQK4ZA%2F20250825%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250825T003024Z&X-Amz-Expires=300&X-Amz-Signature=c9a75de10f67b14328bbd413a6e32edbada8316a4606ce17913dcc93b3020396&X-Amz-SignedHeaders=host">
