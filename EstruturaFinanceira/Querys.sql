USE TesteRodrigoFinanceira

-- Query para retornar os 4 primeiros clientes que está com pelo menos uma parcela atrasada em 5 dias
SELECT TOP 4 *
FROM CLIENTE
WHERE IdCliente IN 
(
	SELECT IdCliente 
	FROM Financiamento
	WHERE IdFinanciamento IN 
	(
		SELECT IdFinanciamento 
		FROM Parcelas
		WHERE DtVencimento < CONVERT(DATE, DATEADD(dd, -5, GETDATE()), 121) AND DtPagamento IS NULL
	)
)

GO

-- Query para retornar clientes que já tiveram 2 ou mais parcelas atrasadas por 10 dias ou mais
WITH TempParcelas AS 
(
	SELECT ROW_NUMBER() OVER (PARTITION BY IdFinanciamento ORDER BY IdFinanciamento ) AS Contador, IdFinanciamento
	FROM Parcelas
	WHERE DATEADD(dd, 10, DtVencimento) < DtPagamento
)

SELECT *
FROM CLIENTE
WHERE IdCliente IN 
(
	SELECT IdCliente 
	FROM Financiamento
	WHERE IdFinanciamento IN (SELECT IdFinanciamento FROM TempParcelas WHERE Contador >= 2) AND ValorTotal >= 10000
)

GO