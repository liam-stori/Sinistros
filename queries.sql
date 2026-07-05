-- 1. Ranking dos ramos com maior percentual de sinistros negados nos últimos 6 meses
SELECT
    a.ramo,
    COUNT(*) AS total_sinistros,
    COUNT(*) FILTER (WHERE s.status = 'Negado') AS total_negados,
    ROUND(
        COUNT(*) FILTER (WHERE s.status = 'Negado') * 100.0
        / COUNT(*),
        2
    ) AS percentual_negados
FROM sinistros s
INNER JOIN apolices a ON a.id = s.apolice_id
WHERE s.data_abertura >= NOW() - INTERVAL '6 months'
GROUP BY a.ramo
ORDER BY percentual_negados DESC;


-- 2. Top 10 clientes com maior soma de ValorEstimado em sinistros em análise ou aprovados
SELECT
    c.id,
    c.nome,
    c.sobrenome,
    c.tipo_documento,
    c.numero_documento,
    SUM(s.valor_estimado) AS total_valor_estimado
FROM sinistros s
INNER JOIN apolices a ON a.id = s.apolice_id
INNER JOIN clientes c ON c.id = a.cliente_id
WHERE s.status IN ('EmAnalise', 'Aprovado')
GROUP BY c.id, c.nome, c.sobrenome, c.tipo_documento, c.numero_documento
ORDER BY total_valor_estimado DESC
LIMIT 10;


-- 3. Tempo médio de resolução (em dias) de sinistros encerrados, agrupado por ramo;
SELECT
    a.ramo,
    ROUND(AVG(EXTRACT(DAY FROM (h.data_alteracao - s.data_abertura))), 2) AS tempo_medio_dias
FROM sinistros s
INNER JOIN apolices a ON a.id = s.apolice_id
INNER JOIN historico_sinistros h 
    ON h.sinistro_id = s.id
    AND h.status_novo  = 'Encerrado'
WHERE s.status = 'Encerrado'
GROUP BY a.ramo
ORDER BY tempo_medio_dias DESC;
