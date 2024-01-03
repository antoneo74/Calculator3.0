let cardItem = document.querySelector("#card-item");

console.log(cardItem.children);


function ShowCard() {

    for (let i = 0; i < cardItem.children.length - 1; i++) {

        for (let j = i + 1; j < cardItem.children.length; j++) {

            let price1 = GetPrice(cardItem.children[i]);

            let price2 = GetPrice(cardItem.children[j]);

            console.log(price1, price2);

            if (price1 > price2) {

                let node = cardItem.replaceChild(cardItem.children[j], cardItem.children[i]);

                cardItem.insertBefore(node, cardItem.children[j]);
            }
        }
    }
}


function GetPrice(item) {

    let price = item;

    console.log(price);

    let priceWithID = price.querySelector("p").innerText;

    let priceOnlyNumber = priceWithID.split(" ");

    return priceOnlyNumber[0];
}