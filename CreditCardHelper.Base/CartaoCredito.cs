using System.ComponentModel;
using System.Text.RegularExpressions;

namespace CreditCardHelper.Base
{
    public readonly struct CartaoCredito
    {
        public String NumeroCartao { get; }
        public String CodigoSeguranca { get; }
        public String ValidadeMes { get; }
        public String ValidadeAno { get; }
        public String NomeTitular { get; }
        public BandeiraCartao Bandeira { get; }
        public bool Valido { get; }

        /// <summary>
        /// Cria a instância de um Cartão de Crédito
        /// </summary>
        /// <param name="numeroCartao"> Numero do cartão, formatado ou não</param>
        /// <param name="codigoSeguranca"> Código de segurança, ou CVV</param>
        /// <param name="validadeMes"> Mês de validade, com dois dígitos</param>
        /// <param name="validadeAno"> Ano de validade, com quatro dígitos</param>
        /// <param name="nomeTitular"> Nome do titular como no cartão</param>
        public CartaoCredito(String numeroCartao, String codigoSeguranca, String validadeMes, String validadeAno, String nomeTitular)
        {
            NumeroCartao = numeroCartao.Replace("-", "").Replace(" ", "");
            Valido = TesteDeLuhn(NumeroCartao);
            Bandeira = ObterBandeira(NumeroCartao);
            CodigoSeguranca = codigoSeguranca;
            ValidadeMes = validadeMes;
            ValidadeAno = validadeAno;
            NomeTitular = nomeTitular;
        }

        private static bool TesteDeLuhn(String numeroCartao)
        {
            int[] digits = new int[numeroCartao.Length];
            for (int len = 0; len < numeroCartao.Length; len++)
            {
                digits[len] = Int32.Parse(numeroCartao.Substring(len, 1));
            }

            int sum = 0;
            bool alt = false;
            for (int i = digits.Length - 1; i >= 0; i--)
            {
                int curDigit = digits[i];
                if (alt)
                {
                    curDigit *= 2;
                    if (curDigit > 9)
                    {
                        curDigit -= 9;
                    }
                }
                sum += curDigit;
                alt = !alt;
            }

            return sum % 10 == 0;
        }

        private static BandeiraCartao ObterBandeira(String numeroCartao)
        {
            if (String.IsNullOrEmpty(numeroCartao) || String.IsNullOrWhiteSpace(numeroCartao))
                return BandeiraCartao.Outra;

            if (Regex.IsMatch(numeroCartao, "^(4011(78|79)|43(1274|8935)|45(1416|7393|763(1|2))|50(4175|6699|67[0-7][0-9]|9000)|50(9[0-9][0-9][0-9])|627780|63(6297|6368)|650(03([^4])|04([0-9])|05(0|1)|05([7-9])|06([0-9])|07([0-9])|08([0-9])|4([0-3][0-9]|8[5-9]|9[0-9])|5([0-9][0-9]|3[0-8])|9([0-6][0-9]|7[0-8])|7([0-2][0-9])|541|700|720|727|901)|65165([2-9])|6516([6-7][0-9])|65500([0-9])|6550([0-5][0-9])|655021|65505([6-7])|6516([8-9][0-9])|65170([0-4]))"))
                return BandeiraCartao.Elo;

            if (Regex.IsMatch(numeroCartao, "^6(?:011|5[0-9]{2})"))
                return BandeiraCartao.Discover;

            if (Regex.IsMatch(numeroCartao, "^606282|^3841(?:[0|4|6]{1})0"))
                return BandeiraCartao.HiperCard;

            if (Regex.IsMatch(numeroCartao, "^3[47]"))
                return BandeiraCartao.AmericanExpress;

            if (Regex.IsMatch(numeroCartao, "^3(?:0[0-5]|[68][0-9])"))
                return BandeiraCartao.Diners;

            if (Regex.IsMatch(numeroCartao, "^((5(([1-2]|[4-5])|0((1|6))|3(0(4((0|[2-9]))|([0-3]|[5-9]))|[1-9])))|((508116))|((502121))|((589916))|(2)|(67)|(506387))") && numeroCartao.Length <= 16)
                return BandeiraCartao.Mastercard;

            if (Regex.IsMatch(numeroCartao, "^4") && numeroCartao.Length <= 16)
                return BandeiraCartao.Visa;

            return BandeiraCartao.Outra;
        }
    }

    public enum BandeiraCartao
    {
        Outra,
        Visa,
        Mastercard,
        Elo,
        HiperCard,
        [Description("American Express")]
        AmericanExpress,
        Discover,
        Diners
    }
}
