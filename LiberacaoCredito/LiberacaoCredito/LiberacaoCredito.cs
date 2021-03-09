using System;
using System.Threading;

namespace LiberacaoCredito
{
    class LiberacaoCredito
    {
        private static Proposta prop;

        static void Main(string[] args)
        {
            TelaBoasVindas();
            IniciaProposta();
        }

        private static void TelaBoasVindas()
        {
            Console.WriteLine("******************************************************************");
            Console.WriteLine("**                                                              **");
            Console.WriteLine("**         Bem vindo ao financiamento do Macedo's Bank          **");
            Console.WriteLine("**                                                              **");
            Console.WriteLine("**                                                              **");
            Console.WriteLine("******************************************************************");
            Console.WriteLine("\nVamos começar fazendo algumas perguntas para gerar sua proposta!");
            Thread.Sleep(2000);
        }

        private static void IniciaProposta()
        {
            prop = new Proposta();

            ObtemValoresEntrada();
            ValidaValorProposta();
            RetornaInformaçõesProposta();
        }

        private static void RetornaInformaçõesProposta()
        {
            prop.StatusProposta = prop.TotalProposta > 1000000 || (prop.TipoCredito == eTipoCredito.PF && prop.ValorCredito < 15000) ? false : true;

            if (prop.StatusProposta)
            {
                Console.WriteLine
                    (
                        "\nSeu crédito foi Aprovado!\n" +
                        $"Valor do crédito financiado: {Math.Round(prop.TotalProposta, 2)}\n" +
                        $"Valor do total do juros: {Math.Round(prop.TaxaJuros, 4)}\n"
                    );
            }
            else
            {
                string novaProposta = string.Empty;

                while (novaProposta == string.Empty)
                {
                    Console.WriteLine("\nInfelizmente Seu crédito foi Reprovado devido ao valor que está em desacordo com as nossas regras de financiamento.\n" +
                                      "Deseja fazer uma nova proposta? (S: Sim, N: Não)");
                    novaProposta = Console.ReadLine().Replace('ã', 'a').ToUpper();

                    if (novaProposta == "S" || novaProposta == "SIM")
                        IniciaProposta();
                    else if (novaProposta == "N" || novaProposta == "NAO")
                        Console.WriteLine("\nTudo bem esperamos nos ver novamente");
                    else
                        novaProposta = string.Empty;

                }
            }
            
            Console.WriteLine("\nMacedo's Bank agradecemos pela confiança, volte sempre!");
        }

        public static void ValidaValorProposta()
        {
            switch (prop.TipoCredito)
            {
                case eTipoCredito.Direto:
                    prop.TaxaJuros =  CalculaJuros(0.02);
                    break;
                case eTipoCredito.Consignado:
                    prop.TaxaJuros = CalculaJuros(0.01);
                    break;
                case eTipoCredito.Imobiliario:
                    prop.TaxaJuros = CalculaJuros(0.05);
                    break;
                case eTipoCredito.PF:
                    prop.TaxaJuros = CalculaJuros(0.03);
                    break;
                case eTipoCredito.PJ:
                    prop.TaxaJuros = CalculaJuros(0.09 / 12);
                    break;
                default:
                    prop.TaxaJuros = 0;
                    break;
            }

            prop.TotalProposta = prop.ValorCredito + prop.TaxaJuros;
        }

        public static double CalculaJuros(double pJuros)
        {
            return Math.Round((prop.ValorCredito * Math.Pow(pJuros + 1, prop.QtdParcelas) * pJuros) / (Math.Pow(pJuros + 1, prop.QtdParcelas) - 1), 4);
        }

        private static void ObtemValoresEntrada()
        {
            while (prop.ValorCredito == 0)
            {
                Console.WriteLine("\nPor favor selecione um valor de Crédito: ");
                double.TryParse(Console.ReadLine(), out double valorCredito);
                prop.ValorCredito = valorCredito;
            }

            while (prop.TipoCredito == 0)
            {
                Console.WriteLine("\nAgora o tipo de Crédito (Direto: 1, Consignado: 2, Imobiliario: 3, Pessoa Física: 4, Pessoa Jurídica: 5): ");
                string sTipoCredito = Console.ReadLine();
                int.TryParse(sTipoCredito, out int iTipoCredito);

                if (sTipoCredito.Replace('í', 'i').Replace(" ", string.Empty).ToUpper() == "PESSOAFISICA")
                    sTipoCredito = "PF";
                
                if (sTipoCredito.Replace('í', 'i').Replace(" ", string.Empty).ToUpper() == "PESSOAJURIDICA")
                    sTipoCredito = "PJ";

                if ((!Enum.IsDefined(typeof(eTipoCredito), sTipoCredito)) && (!Enum.IsDefined(typeof(eTipoCredito), iTipoCredito)))
                    prop.TipoCredito = 0;
                else
                    prop.TipoCredito = (eTipoCredito)Enum.Parse(typeof(eTipoCredito), sTipoCredito);
            }

            while (prop.QtdParcelas == 0)
            {
                Console.WriteLine("\nEm quantas Parcelas deseja fazer (mínima de 5, máxima de 72 parcelas): ");
                int.TryParse(Console.ReadLine(), out int qtdParcelas);

                if (qtdParcelas < 5 || qtdParcelas > 72)
                    prop.QtdParcelas = 0;
                else
                    prop.QtdParcelas = qtdParcelas;
            }

            while (prop.DataPrimeiroVencimento == DateTime.MinValue)
            {
                Console.WriteLine("\nQual a melhor data do Primeiro Vencimento: ");
                string x = Console.ReadLine();

                if (x.Length > 7 && x.Length < 10)
                {
                    int.TryParse(x.Substring(0, 2), out int dia);
                    int.TryParse(x.Substring(2, 2), out int mes);
                    int.TryParse(x.Substring(4, 4), out int ano);

                    x = $"{dia}/{mes}/{ano}";
                }

                DateTime.TryParse(x, out DateTime dataPrimeiroVencimento);
                int qtdDiasVencimento = DateTime.Compare(DateTime.Today, dataPrimeiroVencimento);

                if (dataPrimeiroVencimento <= DateTime.Today)
                    Console.WriteLine("Não é possível gerar com a data de vencimento menor que a data atual.");
                else if (qtdDiasVencimento < 15 && qtdDiasVencimento > 40)
                    prop.DataPrimeiroVencimento = DateTime.MinValue;
                else
                    prop.DataPrimeiroVencimento = dataPrimeiroVencimento;
            }
        }
    }

    public class Proposta
    {
        public double ValorCredito {get; set;}
        public eTipoCredito TipoCredito { get; set; }
        public int QtdParcelas { get; set; }
        public DateTime DataPrimeiroVencimento { get; set; }
        public double TaxaJuros { get; set; }
        public double TotalProposta { get; set; }
        public bool StatusProposta { get; set; }
    }

    public enum eTipoCredito
    {
        Direto = 1,
        Consignado = 2,
        Imobiliario = 3,
        PF = 4,
        PJ = 5
    }
}
