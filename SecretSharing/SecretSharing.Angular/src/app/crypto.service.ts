import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CryptoService {
  private salt: Uint8Array | null = null;
  private iv: Uint8Array | null = null;

  getMessageEncoding(message: string) {
    let enc = new TextEncoder();
    return enc.encode(message);
  }

  /*
  Get some key material to use as input to the deriveKey method.
  The key material is a password supplied by the user.
  */
  getKeyMaterial() {
    let password = "testing123";

    if (!password) {
      throw new Error("No Password was given.");
    }

    let enc = new TextEncoder();
    return window.crypto.subtle.importKey(
      "raw",
      enc.encode(password),
      {name: "PBKDF2"},
      false,
      ["deriveBits", "deriveKey"]
    );
  }

  /*
  Given some key material and some random salt
  derive an AES-GCM key using PBKDF2.
  */
  getKey(keyMaterial: CryptoKey, salt: Uint8Array) {
    const startTime = new Date();
    return window.crypto.subtle.deriveKey(
      {
        "name": "PBKDF2",
        salt: salt,
        "iterations": 210000,
        "hash": "SHA-512"
      },
      keyMaterial,
      { "name": "AES-GCM", "length": 256},
      true,
      [ "encrypt", "decrypt" ]
    ).then(
      x => {
        const endTime = new Date();
        console.log(endTime.getTime() - startTime.getTime());
        return x;
      }
    );
  }

  /*
  Derive a key from a password supplied by the user, and use the key
  to encrypt the message.
  Update the "ciphertextValue" box with a representation of part of
  the ciphertext.
  */
  async encrypt(message: string): Promise<Uint8Array> {
    let keyMaterial = await this.getKeyMaterial();
    this.salt = this.salt ?? window.crypto.getRandomValues(new Uint8Array(16));
    let key = await this.getKey(keyMaterial, this.salt);
    this.iv = this.iv ?? window.crypto.getRandomValues(new Uint8Array(12));
    let encoded = this.getMessageEncoding(message);

    let ciphertext = await window.crypto.subtle.encrypt(
      {
        name: "AES-GCM",
        iv: this.iv
      },
      key,
      encoded
    );

    console.log(new Uint8Array(ciphertext, 0));

    return new Uint8Array(ciphertext, 0);
  }

  /*
  Derive a key from a password supplied by the user, and use the key
  to decrypt the ciphertext.
  If the ciphertext was decrypted successfully,
  update the "decryptedValue" box with the decrypted value.
  If there was an error decrypting,
  update the "decryptedValue" box with an error message.
  */
  async decrypt(ciphertext: Uint8Array) {
    let keyMaterial = await this.getKeyMaterial();
    let key = await this.getKey(keyMaterial, this.salt!);

    try {
      let decrypted = await window.crypto.subtle.decrypt(
        {
          name: "AES-GCM",
          iv: this.iv!
        },
        key,
        ciphertext
      );

      let dec = new TextDecoder();
      return dec.decode(decrypted);
    } catch (e) {
      throw new Error("Failed to decrypt.");
    }
  }
}
