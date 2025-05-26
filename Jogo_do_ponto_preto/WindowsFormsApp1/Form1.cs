using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        // Elementos visuais do jogo
        private Label blinkingLabel;   // Texto inicial piscando para modo perseguição
        private Label dominationLabel; // Texto inicial para modo dominação
        private Timer blinkTimer;      // Temporizador para controle da piscagem do texto
        private PictureBox pictureBox; // Imagem inicial de fundo
        private Button pontoPreto;     // Botão preto que é clicado no modo perseguição
        private Label scoreLabel;      // Exibe a pontuação no canto superior direito
        private Label modeLabel;       // Exibe o modo de jogo selecionado
        private Label filledPercentageLabel; // Exibe a porcentagem de tela preenchida no modo dominação
        private int score;             // Pontuação do jogo
        private bool isTextVisible;    // Controla a visibilidade do texto inicial piscante
        private bool gameStarted;      // Indica se o jogo já começou
        private bool dominationMode;   // Define se o jogo está no modo dominação
        private List<Button> squares;  // Lista de quadrados pretos adicionados ao modo dominação
        private Random rnd;            // Instância única de Random para gerar posições aleatórias

        public Form1()
        {
            InitializeComponent();
            squares = new List<Button>(); // Inicializa a lista de quadrados
            rnd = new Random(); // Instância única de Random
            ConfigurarTelaInicial();
        }

        private void ConfigurarTelaInicial()
        {
            // Configuração básica da tela inicial
            this.Text = "Jogo do Ponto Preto";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.Black;

            // Adiciona imagem de fundo
            pictureBox = new PictureBox
            {
                Image = Image.FromFile(@"C:\Users\guifo\OneDrive\Imagens\Julay.png"),
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            this.Controls.Add(pictureBox);

            // Texto piscante para iniciar o modo perseguição
            blinkingLabel = new Label
            {
                Text = "Press Enter to Play Perseguição",
                Font = new Font("Arial", 36, FontStyle.Bold),
                ForeColor = Color.Black,
                BackColor = Color.White,
                AutoSize = false,
                Size = new Size(500, 100),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point((this.ClientSize.Width - 500) / 2, (this.ClientSize.Height - 100) / 2 - 50)
            };
            this.Controls.Add(blinkingLabel);
            blinkingLabel.BringToFront();

            // Texto para iniciar modo dominação
            dominationLabel = new Label
            {
                Text = "Press Space to Play Domination",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.Black,
                BackColor = Color.White,
                AutoSize = false,
                Size = new Size(500, 60),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point((this.ClientSize.Width - 500) / 2, (this.ClientSize.Height - 60) / 2 + 60)
            };
            this.Controls.Add(dominationLabel);
            dominationLabel.BringToFront();

            // Configuração do temporizador para texto piscante
            blinkTimer = new Timer { Interval = 500 };
            blinkTimer.Tick += BlinkTimer_Tick;
            blinkTimer.Start();

            this.KeyDown += Form1_KeyDown;
        }

        private void BlinkTimer_Tick(object sender, EventArgs e)
        {
            // Alterna a visibilidade dos textos iniciais para criar efeito de piscar
            isTextVisible = !isTextVisible;
            blinkingLabel.Visible = isTextVisible;
            dominationLabel.Visible = isTextVisible;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!gameStarted)
            {
                // Oculta os textos e interrompe o temporizador ao iniciar o jogo
                blinkingLabel.Visible = false;
                dominationLabel.Visible = false;
                blinkTimer.Stop();

                // Inicia o jogo dependendo da tecla pressionada
                if (e.KeyCode == Keys.Enter)
                {
                    StartGame("Modo Perseguição");
                }
                else if (e.KeyCode == Keys.Space)
                {
                    StartGame("Modo Dominação");
                }
            }
            else if (dominationMode && e.KeyCode == Keys.Space)
            {
                // No modo dominação, a tecla 'espaço' cria novos quadrados
                CriarNovoQuadrado();
                AtualizarPorcentagem();
            }
        }

        private void StartGame(string mode)
        {
            gameStarted = true;
            dominationMode = mode == "Modo Dominação";
            pictureBox.Visible = false;
            this.BackColor = Color.Blue;

            // Exibe o modo de jogo atual
            modeLabel = new Label
            {
                Text = mode,
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point((this.ClientSize.Width - 100) / 2, 50)
            };
            this.Controls.Add(modeLabel);
            modeLabel.BringToFront();

            score = 0;

            // Exibe pontuação no canto superior direito
            scoreLabel = new Label
            {
                Text = "Pontuação: 0",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(this.ClientSize.Width - 250, 20)
            };
            this.Controls.Add(scoreLabel);
            scoreLabel.BringToFront();

            if (dominationMode)
            {
                // Exibe porcentagem de tela preenchida
                filledPercentageLabel = new Label
                {
                    Text = "Tela preenchida: 0%",
                    Font = new Font("Arial", 24, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = Color.Transparent,
                    AutoSize = true,
                    Location = new Point(this.ClientSize.Width - 250, 60)
                };
                this.Controls.Add(filledPercentageLabel);
                filledPercentageLabel.BringToFront();
            }
            else
            {
                // Cria o botão preto para ser clicado no modo perseguição
                pontoPreto = new Button
                {
                    Size = new Size(20, 20),
                    BackColor = Color.Black,
                    FlatStyle = FlatStyle.Flat
                };
                pontoPreto.FlatAppearance.BorderSize = 0;
                pontoPreto.Click += PontoPreto_Click;
                this.Controls.Add(pontoPreto);
            }
        }

        private void PontoPreto_Click(object sender, EventArgs e)
        {
            // Incrementa a pontuação quando o ponto preto é clicado
            score++;
            scoreLabel.Text = "Pontuação: " + score;

            // Move o ponto preto para uma nova posição aleatória
            int x = rnd.Next(0, this.ClientSize.Width - pontoPreto.Width);
            int y = rnd.Next(0, this.ClientSize.Height - pontoPreto.Height);
            pontoPreto.Location = new Point(x, y);
        }

        private void CriarNovoQuadrado()
        {
            // Adiciona um novo quadrado preto aleatório na tela no modo dominação
            Button novoQuadrado = new Button
            {
                Size = new Size(30, 30),
                BackColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            novoQuadrado.FlatAppearance.BorderSize = 0;

            int x = rnd.Next(0, this.ClientSize.Width - novoQuadrado.Width);
            int y = rnd.Next(0, this.ClientSize.Height - novoQuadrado.Height);
            novoQuadrado.Location = new Point(x, y);

            this.Controls.Add(novoQuadrado);
            squares.Add(novoQuadrado);

            AtualizarPorcentagem();
        }

        private void AtualizarPorcentagem()
        {
            // Calcula a área total ocupada pelos quadrados no modo dominação
            int totalPixels = this.ClientSize.Width * this.ClientSize.Height;
            int occupiedPixels = squares.Sum(square => square.Width * square.Height);
            double percentageFilled = (double)occupiedPixels / totalPixels * 100;

            // Atualiza as labels com valores precisos
            filledPercentageLabel.Text = $"Tela preenchida: {percentageFilled:F2}%";
            scoreLabel.Text = $"Pontuação: {percentageFilled:F2}%";
        }
    }
}
