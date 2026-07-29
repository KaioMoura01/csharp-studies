using BankLedgerApi.Enums;
using BankLedgerApi.Models;
using AwesomeAssertions;

namespace BanLedgerApiXUnitTests.UnitTests;

public class TaxDocumentTests
{
    [Fact]
    public void Constructor_WithValidCpf_SetsNumberAndType()
    {
        var document = new TaxDocument("12345678901", DocumentTypeEnum.Cpf);

        document.Number.Should().Be("12345678901");
        document.Type.Should().Be(DocumentTypeEnum.Cpf);
    }

    [Fact]
    public void Constructor_WithValidCnpj_SetsNumberAndType()
    {
        var document = new TaxDocument("12345678000199", DocumentTypeEnum.Cnpj);

        document.Number.Should().Be("12345678000199");
        document.Type.Should().Be(DocumentTypeEnum.Cnpj);
    }

    [Fact]
    public void Constructor_StripsNonDigitCharacters()
    {
        var document = new TaxDocument("529.982.247-25", DocumentTypeEnum.Cpf);

        document.Number.Should().Be("52998224725");
    }

    [Theory]
    [InlineData("123", DocumentTypeEnum.Cpf)]
    [InlineData("123456789012", DocumentTypeEnum.Cpf)]
    [InlineData("12345678000", DocumentTypeEnum.Cnpj)]
    [InlineData("123456780001999", DocumentTypeEnum.Cnpj)]
    public void Constructor_WithWrongDigitCount_Throws(string number, DocumentTypeEnum type)
    {
        var act = () => new TaxDocument(number, type);

        act.Should().Throw<ArgumentException>();
    }
}
