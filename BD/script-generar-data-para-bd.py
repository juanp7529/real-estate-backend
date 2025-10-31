import json, random, datetime

cities = ["Medellín", "Bogotá", "Cali", "Barranquilla", "Cartagena", "Bucaramanga"]
names = ["Apartamento", "Casa", "Penthouse", "Finca", "Oficina", "Loft", "Chalet", "Villa"]

data = []
for i in range(1, 51):
    city = random.choice(cities)
    kind = random.choice(names)
    price = random.randint(200, 1200) * 1_000_000
    year = random.randint(2015, 2024)
    data.append({
        "Name": f"{kind} {city} #{i}",
        "Address": f"Calle {random.randint(10, 100)} #{random.randint(1, 99)}-{random.randint(1, 99)}, {city}",
        "Price": price,
        "CodeInternal": f"{kind[:2].upper()}-{i:03}",
        "Year": year,
        "Owner": {
            "OwnerId": f"{i}",
            "Name": f"Propietario {i}",
            "Address": f"Dirección {i}, {city}",
            "Photo": f"https://picsum.photos/200/300",
            "Birthday": {"$date": str(datetime.date(1980 + (i % 20), random.randint(1, 12), random.randint(1, 28))) + "T00:00:00Z"}
        },
        "Images": [{
            "ImageId": f"{i}",
            "File": f"https://picsum.photos/200/300",
            "Enabled": random.choice([True, False])
        }],
        "Traces": [{
            "TraceId": f"{i}",
            "Name": "Venta inicial",
            "DateSale": {"$date": str(datetime.date(2020 + (i % 4), random.randint(1, 12), random.randint(1, 28))) + "T00:00:00Z"},
            "Value": price - random.randint(10, 50) * 1_000_000,
            "Tax": random.randint(5, 15) * 1_000_000
        }]
    })

with open("properties.json", "w", encoding="utf-8") as f:
    json.dump(data, f, ensure_ascii=False, indent=2)
