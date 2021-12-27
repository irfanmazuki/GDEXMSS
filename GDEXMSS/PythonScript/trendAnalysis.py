from pytrends.request import TrendReq
import sys

pytrends = TrendReq(hl='en-us')

num1 = str(sys.argv[1])
all_keywords = [num1]
keywords = []
cat='0'
geo = ''
gprop = ''

def check_trends():
  pytrends.build_payload(all_keywords, cat, timeframe='today 5-y', geo='', gprop='')
  data = pytrends.interest_over_time()
  mean = round(data.mean(), 2)
  avg = round(data[kw][-52:].mean(),2) #Last year average
  avg2 = round(data[kw][:52].mean(),2) #Yearly avaerage of 5 years ago
  trend = round(((avg/mean[kw])-1)*100,2)
  trend2 = round(((avg/avg2)-1)*100,2)
  print('The average 5 years interest of ' + kw + ' was ' + str(mean[kw]) + ' <br>')
  print('The last year interest of ' + kw + ' compared to the last 5 years' + ' has changed by ' + str(trend) + '% <br>')
  #stable trends
  if mean[kw] > 75 and abs(trend) <= 75:
    print('The interest for ' + kw + ' is stable in the last 5 years')
  elif mean[kw] > 75 and trend > 5:
    print('The interest for ' + kw + ' is stable and increasing in the last 5 years.')
  elif mean[kw] > 75 and trend > -5:
    print('The interest for ' + kw + ' is stable and decreasing in the last 5 years.')

  #relatively stable
  elif mean[kw] > 60 and abs(trend) <= 15:
    print('The interest for ' + kw + ' is relatively stable in the last 5 years.')
  elif mean[kw] > 60 and trend > 15:
    print('The interest for ' + kw + ' is relatively stable and increasing in the last 5 years.')
  elif mean[kw] > 60 and trend > -15:
    print('The interest for ' + kw + ' is relatively stable and decreasing in the last 5 years.')

  #seasonal
  elif mean[kw] > 20 and abs(trend) > 15:
    print('The interest for ' + kw + ' is seasonal.')

  #trending product -> low mean 
  elif mean[kw] > 20 and trend > 15:
    print('The interest for ' + kw + ' is trending.')
  
  #Declining keywords
  elif mean[kw] > 20 and trend < -15:
    print('The interest for ' + kw + ' is significantly decreasing')

  #cyclical
  elif mean[kw] > 5 and abs(trend) <= 15:
    print('The interest for ' + kw + ' is cyclical.')
  
  #new
  elif mean[kw] > 0 and trend > 15:
    print('The interest for ' + kw + ' is new and trending.')

  #Declining
  elif mean[kw] > 0 and trend < -15:
    print('The interest for ' + kw + ' is declining and not comparable to its peak.')

  print('')

for kw in all_keywords:
  keywords.append(kw)
  check_trends()
  keywords.pop()
