去年度損益累計差異% = 
if(ISFILTERED('工作表1'[YYYYMM]) || ISFILTERED('工作表1'[Current]),
((TOTALYTD(sum('工作表1'[合併損益]),DATESYTD('工作表1'[項目]))-CALCULATE(sum('工作表1'[合併損益]),DATEADD(DATESYTD('工作表1'[項目]),-1,YEAR)))
    /CALCULATE(sum('工作表1'[合併損益]),DATEADD(DATESYTD('工作表1'[項目]),-1,YEAR)))
,
((TOTALYTD(sum('工作表1'[合併損益]),DATESYTD('工作表1'[項目]))-CALCULATE(sum('工作表1'[合併損益]),PREVIOUSYEAR('工作表1'[項目])))
/CALCULATE(sum('工作表1'[合併損益]),PREVIOUSYEAR('工作表1'[項目]))))